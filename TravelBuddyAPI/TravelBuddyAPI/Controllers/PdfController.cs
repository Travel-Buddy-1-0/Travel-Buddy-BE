using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;
using System.Diagnostics;
using System.Text.Json;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly PuppeteerSharpPDFServices _pdfService;

        private readonly SupabaseService _supabaseService;
        private readonly ILogger<PdfController> _logger;
        private readonly QuestPdfService _questPdfService;
        public PdfController(

            PuppeteerSharpPDFServices pdfService,
            SupabaseService supabaseService,
            QuestPdfService questPdfService,
            ILogger<PdfController> logger)
        {

            _pdfService = pdfService;
            _supabaseService = supabaseService;
            _logger = logger;
            _questPdfService = questPdfService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAndUpload([FromBody] PdfRequest request)
        {
            if (string.IsNullOrEmpty(request.HtmlContent) || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("Thiếu HTML Content hoặc UserId.");
            }

            try
            {
                // BƯỚC 1: Generate PDF từ HTML (PuppeteerSharp)
                _logger.LogInformation("Bắt đầu tạo PDF...");
                byte[] pdfBytes = await _pdfService.GeneratePdfAsync(request.HtmlContent);

                // BƯỚC 2: Tạo tên file duy nhất
                // Ví dụ: cv_user123_1711234567.pdf
                string fileName = $"cv_{request.UserId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.pdf";

                // BƯỚC 3: Upload lên Supabase
                _logger.LogInformation("Bắt đầu upload lên Cloud...");
                string? publicUrl = await _supabaseService.UploadFileAsync(request.UserId, pdfBytes, fileName);

                if (string.IsNullOrEmpty(publicUrl))
                {
                    return StatusCode(500, "Tạo PDF thành công nhưng Upload thất bại.");
                }

                // BƯỚC 4: Trả kết quả về FE
                return Ok(new
                {
                    Success = true,
                    Url = publicUrl,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi quy trình tạo CV");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



        [HttpGet("generate")]
        public async Task<IActionResult> GenerateQuestPdf() // Không cần [FromBody] nữa
        {
            try
            {
                // 1. Bắt đầu đo giờ
                var sw = Stopwatch.StartNew();
                _logger.LogInformation("🚀 [QuestPDF] Bắt đầu tạo PDF từ file JSON local...");

                // --- ĐỌC DATA TỪ FILE JSON ---
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "mockCvData.json");
                if (!System.IO.File.Exists(filePath))
                {
                    return BadRequest($"Không tìm thấy file 'mockCvData.json' tại {filePath}. Vui lòng copy file JSON vào thư mục gốc của Backend.");
                }

                var jsonString = await System.IO.File.ReadAllTextAsync(filePath);
                var request = JsonSerializer.Deserialize<CvGenerationRequest>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null) return BadRequest("Lỗi format JSON trong file.");
                // ----------------------------

                // --- HARDCODE TEST DATA ---
                // Ghi đè Avatar URL để test ảnh cụ thể này
                if (request.DataJson?.Profile != null)
                {
                    request.DataJson.Profile.Avatar = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQoKZ5ev0ls_lfr-UBnDRwp5-Jh2u3INVeJig&s";
                }
                // --------------------------

                // 2. Gọi Service để vẽ PDF (Cực nhanh)
                var pdfBytes = await _questPdfService.GenerateCvFromDataAsync(request);

                // 3. Upload lên Supabase (Hardcode UserID = 999)
                string userId = "999";
                string fileName = $"cv_quest_{userId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.pdf";

                _logger.LogInformation("☁️ Đang upload lên Supabase...");
                string? publicUrl = await _supabaseService.UploadFileAsync(userId, pdfBytes, fileName);

                // 4. Kết thúc đo giờ
                sw.Stop();
                _logger.LogInformation($"✅ [QuestPDF] Hoàn tất! Tổng thời gian: {sw.ElapsedMilliseconds} ms");

                if (string.IsNullOrEmpty(publicUrl))
                {
                    return StatusCode(500, "Tạo PDF thành công nhưng Upload thất bại.");
                }

                // 5. Trả về kết quả
                return Ok(new
                {
                    Success = true,
                    Url = publicUrl,
                    FileName = fileName,
                    Engine = "QuestPDF",
                    TimeMs = sw.ElapsedMilliseconds // Trả về thời gian cho FE hiển thị
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi QuestPDF");
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

    }


    // Model nhận dữ liệu từ FE
    public class PdfRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
    }
}
