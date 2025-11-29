using Microsoft.AspNetCore.Mvc;
using Services.Implement;
using System.Diagnostics;
using System.Text.Json;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/cv-parser")]
    public class CvParserController : ControllerBase
    {
        private readonly FileParserService _fileParser;
        private readonly GeminiService _geminiService;
        private readonly ILogger<CvParserController> _logger;

        public CvParserController(
            FileParserService fileParser,
            GeminiService geminiService,
            ILogger<CvParserController> logger)
        {
            _fileParser = fileParser;
            _geminiService = geminiService;
            _logger = logger;
        }

        // [POST] api/cv-parser/analyze-match
        [HttpPost("analyze-match")]
        public async Task<IActionResult> AnalyzeCvWithJd([FromForm] AnalyzeCvRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("🚀 Bắt đầu quy trình Phân tích CV vs JD...");

            // 1. Validate Input
            if (request.CvFile == null || request.CvFile.Length == 0) return BadRequest("Thiếu file CV.");
            if (request.JdFile == null || request.JdFile.Length == 0) return BadRequest("Thiếu file JD.");

            try
            {
                // 2. Parse File -> Text (Xử lý song song)
                // Lưu ý: Dùng Task.Run để đẩy việc nặng sang Thread khác, tránh block Main Thread
                var cvTask = Task.Run(() => ParseFileToString(request.CvFile));
                var jdTask = Task.Run(() => ParseFileToString(request.JdFile));

                await Task.WhenAll(cvTask, jdTask);

                string cvText = await cvTask;
                string jdText = await jdTask;

                if (string.IsNullOrWhiteSpace(cvText)) return BadRequest($"Lỗi: Không đọc được nội dung file CV ({request.CvFile.FileName})");
                if (string.IsNullOrWhiteSpace(jdText)) return BadRequest($"Lỗi: Không đọc được nội dung file JD ({request.JdFile.FileName})");

                // 3. Gọi Gemini Service (Chỉ gửi Text -> Tiết kiệm Token)
                _logger.LogInformation($"Gửi dữ liệu sang Gemini (Lang: {request.OutputLanguage})...");

                // Gọi Service mới (Trả về GeminiResult object)
                var result = await _geminiService.OptimizeCvWithJdAsync(cvText, jdText, request.OutputLanguage);

                if (result == null || string.IsNullOrEmpty(result.TextContent))
                {
                    return StatusCode(500, "Gemini không trả về kết quả.");
                }

                // 4. Parse JSON string từ Gemini thành Object để trả về Frontend
                object dataResponse;
                try
                {
                    // Cố gắng parse string JSON thành object
                    dataResponse = JsonSerializer.Deserialize<object>(result.TextContent);
                }
                catch
                {
                    // Nếu Gemini trả về text thường, giữ nguyên
                    dataResponse = result.TextContent;
                }

                stopwatch.Stop();
                var totalTime = stopwatch.ElapsedMilliseconds;
                _logger.LogInformation($"✅ Hoàn tất. Tổng thời gian: {totalTime} ms");

                return Ok(new
                {
                    success = true,
                    totalTimeMs = totalTime,
                    // Trả về thông tin Token để bạn kiểm tra (Input/Output/Total)
                    billing = result.Usage,
                    data = dataResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Controller AnalyzeCvWithJd");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // --- HÀM HELPER DUY NHẤT (Gọn gàng, sạch sẽ) ---
        private string ParseFileToString(IFormFile file)
        {
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                // Dùng OpenReadStream tiết kiệm bộ nhớ hơn là CopyTo MemoryStream
                using var stream = file.OpenReadStream();

                if (ext == ".pdf")
                {
                    return _fileParser.ParsePdf(stream);
                }
                else if (ext == ".docx" || ext == ".doc")
                {
                    return _fileParser.ParseDocx(stream);
                }
                else if (ext == ".txt")
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
                else
                {
                    _logger.LogWarning($"Bỏ qua file không hỗ trợ: {file.FileName}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi parse file {file.FileName}");
                return string.Empty;
            }
        }
    }

    // DTO giữ nguyên
    public class AnalyzeCvRequestDto
    {
        public IFormFile CvFile { get; set; }
        public IFormFile JdFile { get; set; }
        public string OutputLanguage { get; set; } = "vi";
    }
}
