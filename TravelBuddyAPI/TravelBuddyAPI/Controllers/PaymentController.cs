using BusinessObject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PayOsService _payOsService;

        public PaymentController(PayOsService payOsService)
        {
            _payOsService = payOsService;
        }

        // 📤 Tạo link thanh toán
        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request)
        {
            var url = await _payOsService.CreatePaymentLink(
                description: request.Description,
                amount: request.Amount
            );

            return Ok(new { paymentUrl = url });
        }

        // 📥 Nhận callback thanh toán từ PayOS
        [HttpPost("webhook")]
        public async Task<IActionResult> PayOsWebhook()
        {
            // ✅ Lấy raw body để xác minh chữ ký
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            var signature = Request.Headers["x-signature"].FirstOrDefault();
            if (signature == null)
                return BadRequest("Missing signature header.");

            // ✅ Kiểm tra chữ ký xem có hợp lệ không
            var isValid = _payOsService.VerifySignature(rawBody, signature);
            if (!isValid)
                return Unauthorized("Invalid signature.");

            // ✅ Nếu chữ ký hợp lệ → xử lý nội dung webhook
            Console.WriteLine("✅ Webhook received & verified: " + rawBody);

            // TODO: Cập nhật trạng thái đơn hàng trong DB tại đây
            return Ok();
        }
    }
}
