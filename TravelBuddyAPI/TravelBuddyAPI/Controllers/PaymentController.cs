using BusinessLogic.Services;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Services;
using System.Text.Json;


namespace TravelBuddyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PayOsService _payOsService;
        private readonly IPaymentHistoryService _paymentService;
        private readonly IUserService _userService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PayOsService payOsService, IPaymentHistoryService paymentService, IUserService userService, ILogger<PaymentController> logger)
        {
            _payOsService = payOsService;
            _paymentService = paymentService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request)
        {
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var payment = new PaymentHistory
            {
                UserId = request.UserId,
                TransactionCode = orderCode,
                Amount = request.Amount,
                Status = "PENDING",
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };
            _logger.LogInformation($"Tạo payment record: UserId={payment.UserId}, Amount={payment.Amount}, OrderCode={payment.TransactionCode}");
            await _paymentService.AddAsync(payment);
            var url = await _payOsService.CreatePaymentLink(
                description: request.Description,
                amount: request.Amount,
                orderCode: orderCode
            );
          

            return Ok(new { paymentUrl = url });
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> PayOsWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();

                _logger.LogInformation($"PayOS Webhook received. Raw Body: {rawBody}");
                // var signature = Request.Headers["x-signature"].FirstOrDefault();                
                var payload = JsonSerializer.Deserialize<WebhookType>(rawBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Giúp khớp với các thuộc tính camelCase
                });
                if (payload == null || payload.data == null)
                {
                    _logger.LogWarning("Webhook payload is invalid or empty.");
                    return BadRequest("Invalid payload.");
                }

                if (!_payOsService.VerifyWebhookSignature(payload))
                {
                    _logger.LogWarning("Webhook signature verification failed.");
                    return Unauthorized();
                }
                var orderCode = payload.data.orderCode;
                var payment = await _paymentService.GetByOrderCodeAsync(orderCode);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment record not found for order code: {orderCode}");
                    return NotFound();
                }
                if (payment.Status == "PAID")
                {
                    _logger.LogInformation($"Webhook for order code {orderCode} has already been processed.");
                    return Ok();
                }

                string payOsStatusCode = payload.data.code;
                if (payOsStatusCode == "00")
                {
                    try
                    {
                        await _userService.Deposit(payment.UserId, payment.Amount);
                        payment.Status = "PAID";
                        payment.UpdatedAt = DateTime.UtcNow;
                        await _paymentService.UpdateAsync(payment);

                        _logger.LogInformation($"💰 User {payment.UserId} has successfully PAID for transaction {orderCode}. Both deposit and status update were successful.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"CRITICAL: Deposit failed for user {payment.UserId} with order code {orderCode}. The payment status remains PENDING.");
                        return Ok(new { error = 1, message = "Deposit failed but webhook acknowledged." });
                    }
                }
                else 
                {
                    payment.Status = "FAILED";
                    payment.UpdatedAt = DateTime.UtcNow;
                    await _paymentService.UpdateAsync(payment);
                    _logger.LogInformation($"Transaction {orderCode} for user {payment.UserId} has FAILED as per PayOS webhook.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LỖI NỘI BỘ (500) khi xử lý PayOS Webhook.");
                return Ok(new { error = 1, message = "Internal error, but webhook acknowledged. /n" + ex });
            }
        }

    }
}
