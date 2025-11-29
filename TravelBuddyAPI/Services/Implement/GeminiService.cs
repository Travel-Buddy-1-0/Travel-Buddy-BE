using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;

        // Lưu ý: Dùng flash cho rẻ, pro cho thông minh.
        private const string MODEL_ID = "gemini-2.5-flash";

        public GeminiService(HttpClient httpClient, IConfiguration config, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _apiKey = config["Gemini:ApiKey"]?.Trim() ?? throw new ArgumentNullException("ApiKey missing");
            _logger = logger;
        }

        // --- METHOD 1: OCR (Nếu bạn dùng UglyToad thì KHÔNG CẦN gọi hàm này nữa) ---
        //public async Task<GeminiResult?> ParseCvToTextAsync(byte[] fileData, string fileName)
        //{
        //    var mimeType = GetMimeType(fileName);
        //    if (mimeType == null) return null;

        //    var requestUri = new Uri($"https://generativelanguage.googleapis.com/v1beta/models/{MODEL_ID}:generateContent?key={_apiKey}");
        //    var prompt = AiPrompts.CvParserPrompt;

        //    var payload = new
        //    {
        //        contents = new[]
        //        {
        //            new
        //            {
        //                parts = new object[]
        //                {
        //                    new { text = prompt },
        //                    new
        //                    {
        //                        inline_data = new
        //                        {
        //                            mime_type = mimeType,
        //                            data = Convert.ToBase64String(fileData)
        //                        }
        //                    }
        //                }
        //            }
        //        },
        //        generationConfig = new { temperature = 0.1, responseMimeType = "text/plain" }
        //    };

        //    return await CallGeminiApiAsync(payload, requestUri);
        //}

        // --- METHOD 2: Matching/Optimize (QUAN TRỌNG: CHỈ GỬI TEXT) ---
        public async Task<GeminiResult?> OptimizeCvWithJdAsync(string cvRawJson, string jdText, string outputLanguage)
        {
            var requestUri = new Uri($"https://generativelanguage.googleapis.com/v1beta/models/{MODEL_ID}:generateContent?key={_apiKey}");

            var finalPrompt = $@"
{AiPrompts.CvOptimizerPrompt}

--------------------------------------------------
HERE IS THE CANDIDATE'S CURRENT CV DATA (TEXT/JSON):
{cvRawJson}

--------------------------------------------------
HERE IS THE JOB DESCRIPTION (JD):
{jdText}

--------------------------------------------------
IMPORTANT: ALL THE OPTIMIZED CONTENT MUST BE GENERATED IN THE FOLLOWING LANGUAGE: {outputLanguage}
";

            // CHỈNH SỬA QUAN TRỌNG: Payload chỉ chứa Text, không chứa inline_data
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = finalPrompt } } }
                },
                generationConfig = new
                {
                    temperature = 0.3,
                    responseMimeType = "application/json"
                }
            };

            return await CallGeminiApiAsync(payload, requestUri);
        }

        // --- CORE: Hàm gọi API trả về GeminiResult ---
        private async Task<GeminiResult?> CallGeminiApiAsync(object payload, Uri requestUri)
        {
            var jsonBody = JsonSerializer.Serialize(payload);
            _logger.LogWarning("📦 DEBUG PAYLOAD: {Json}", jsonBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                var responseString = await response.Content.ReadAsStringAsync();
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API Error: {Status} - {Body}", response.StatusCode, responseString);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseString);
                var result = new GeminiResult();


                if (doc.RootElement.TryGetProperty("usageMetadata", out var usage))
                {
                    result.Usage.InputTokens = usage.TryGetProperty("promptTokenCount", out var p) ? p.GetInt32() : 0;
                    result.Usage.OutputTokens = usage.TryGetProperty("candidatesTokenCount", out var c) ? c.GetInt32() : 0;
                    result.Usage.TotalTokens = usage.TryGetProperty("totalTokenCount", out var t) ? t.GetInt32() : 0;

                    Debug.WriteLine(usage.GetRawText());
                    _logger.LogInformation("💰 Billing: {Total} (In: {In}, Out: {Out})", result.Usage.TotalTokens, result.Usage.InputTokens, result.Usage.OutputTokens);
                }

                // 2. Lấy Text Content
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    result.TextContent = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString()?.Trim();
                    Debug.WriteLine(result.TextContent);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception calling Gemini API");
                return null;
            }
        }

        // Hàm CountTokens giữ nguyên hoặc bỏ qua nếu không dùng
        // Hàm GetMimeType giữ nguyên
        private static string? GetMimeType(string fileName)
        {
            // ... (Code cũ của bạn)
            return "application/pdf"; // Ví dụ rút gọn
        }
    }
}
public class GeminiResult
{
    public string? TextContent { get; set; }
    public TokenUsage Usage { get; set; } = new();
}

public class TokenUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
}