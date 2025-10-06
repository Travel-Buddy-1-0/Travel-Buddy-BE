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

        public PaymentController(PayOsService payOsService)
        {
            _payOsService = payOsService;
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
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            var signature = Request.Headers["x-signature"].FirstOrDefault();
            if (!_payOsService.VerifySignature(rawBody, signature))
                return Unauthorized();

            var payload = JsonSerializer.Deserialize<PayOsWebhookPayload>(rawBody);
            var orderCode = payload.orderCode;

            // ✅ Tìm bản ghi trong DB
            var payment = await _paymentService.GetByOrderCodeAsync(orderCode);
            if (payment == null) return NotFound();

            // ✅ Cập nhật trạng thái
            payment.Status = payload.status; // "PAID" hoặc "CANCELLED"
            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentService.UpdateAsync(payment);
            await _userService.Deposit(payment.UserId, payment.Amount);
            //Console.WriteLine($"💰 User {payment.UserId} đã {payment.Status} giao dịch {orderCode}");
            return Ok();
        }

    }
}
