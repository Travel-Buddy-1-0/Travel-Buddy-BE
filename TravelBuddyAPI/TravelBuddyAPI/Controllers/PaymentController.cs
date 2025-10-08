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
            // Bắt đầu khối try-catch để đảm bảo không bị lỗi 500
            try
            {
                // 1. Đọc Raw Body
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();

                _logger.LogInformation($"PayOS Webhook received. Raw Body: {rawBody}");

                // 2. Xác thực Signature
                var signature = Request.Headers["x-signature"].FirstOrDefault();
                if (!_payOsService.VerifySignature(rawBody, signature))
                {
                    _logger.LogWarning("Webhook signature verification failed.");
                    return Unauthorized(); // Trả về 401 nếu xác thực thất bại
                }

                // 3. Deserialization và Logic nghiệp vụ
                var payload = JsonSerializer.Deserialize<PayOsWebhookPayload>(rawBody);
                var orderCode = payload.orderCode;

                var payment = await _paymentService.GetByOrderCodeAsync(orderCode);
                if (payment == null)
                {
                    _logger.LogWarning($"OrderCode {orderCode} not found in DB.");
                    return Ok(); // Trả về Ok để PayOS không gửi lại, dù không tìm thấy
                }

                // Cập nhật trạng thái
                payment.Status = payload.status;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentService.UpdateAsync(payment);

                // Cập nhật tiền (Nếu lỗi ở đây, bạn sẽ bị 500)
                await _userService.Deposit(payment.UserId, payment.Amount);

                _logger.LogInformation($"💰 User {payment.UserId} đã {payment.Status} giao dịch {orderCode} thành công.");

                // 4. Trả về 200 OK
                return Ok();
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi 500
                _logger.LogError(ex, "LỖI NỘI BỘ (500) khi xử lý PayOS Webhook.");

                // QUAN TRỌNG: Trả về 200 OK để PayOS ngừng thử lại, 
                // sau đó bạn có thể kiểm tra Log để khắc phục lỗi.
                return Ok(new { error = 1, message = "Internal error, but webhook acknowledged." });
            }
        }

    }
}
