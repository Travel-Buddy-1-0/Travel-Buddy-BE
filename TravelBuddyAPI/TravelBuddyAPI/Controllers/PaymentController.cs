using BusinessLogic.Services;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                //var signature = Request.Headers["x-signature"].FirstOrDefault();                
                var payload = JsonSerializer.Deserialize<PayOsWebhookPayload>(rawBody);
                //_logger.LogInformation(payload);
                if (!_payOsService.VerifySignature(rawBody, payload.signature))
                {
                    _logger.LogWarning("Webhook signature verification failed.");
                    return Unauthorized();
                }
                var orderCode = payload.data.orderCode;
                string payOsStatusCode = payload.data.code;
                string statusForDb = (payOsStatusCode == "00") ? "PAID" : "FAILED"; 

                var payment = await _paymentService.GetByOrderCodeAsync(orderCode);
                if (payment == null) return NotFound();

                payment.Status = statusForDb;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentService.UpdateAsync(payment);

                await _userService.Deposit(payment.UserId, payment.Amount);

                _logger.LogInformation($"💰 User {payment.UserId} đã {payment.Status} giao dịch {orderCode} thành công.");

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
