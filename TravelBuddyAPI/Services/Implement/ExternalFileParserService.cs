using Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Services.Implement
{
    public class ExternalFileParserService : IFileParserService
    {
        private readonly HttpClient _httpClient;

        // HttpClient được inject từ DI Container, đã cấu hình BaseAddress
        public ExternalFileParserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);

            // Cần reset vị trí stream về 0 nếu nó đã bị đọc trước đó
            if (fileStream.CanSeek) fileStream.Position = 0;

            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // Hoặc lấy từ tham số nếu cần
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("/extract-text", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ExtractResponse>();
            return result?.Text ?? string.Empty;
        }

        private class ExtractResponse
        {
            public string Text { get; set; }
        }
    }
}
