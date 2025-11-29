using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class SupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _serviceRoleKey;
        private readonly string _bucketName;
        private readonly string _anonKey;
        private readonly ILogger<SupabaseService> _logger;

        public SupabaseService(HttpClient httpClient, IConfiguration config, ILogger<SupabaseService> logger)
        {
            _httpClient = httpClient;
            _supabaseUrl = config["Supabase:Url"]!;
            _serviceRoleKey = config["Supabase:ServiceRoleKey"]!;
            _bucketName = config["Supabase:BucketName"]!;
            _logger = logger;
            _anonKey = config["Supabase:AnonKey"]!;
        }

        private void SetAuthHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            // Dùng ServiceRoleKey để có quyền đọc mọi file (kể cả trong folder private của user)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _serviceRoleKey);
            _httpClient.DefaultRequestHeaders.Add("apikey", _serviceRoleKey);
        }

        // --- Hàm cũ: Tạo Signed URL Upload ---
        public async Task<string?> CreateUploadSignedUrlAsync(string filePath, int expiresInSeconds = 60)
        {
            SetAuthHeaders();

            var body = JsonSerializer.Serialize(new { expiresIn = expiresInSeconds });
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

            var url = $"{_supabaseUrl}/storage/v1/object/upload/sign/{_bucketName}/{filePath}";

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to create upload signed URL. Status: {Status}, Body: {Body}",
                     response.StatusCode, responseBody);
                return null;
            }

            using var doc = JsonDocument.Parse(responseBody);

            if (doc.RootElement.TryGetProperty("url", out var signedUrlElement))
            {
                return _supabaseUrl + "/storage/v1" + signedUrlElement.GetString();
            }

            if (doc.RootElement.TryGetProperty("signedURL", out var oldSignedUrlElement))
            {
                return oldSignedUrlElement.GetString();
            }

            return null;
        }

        public async Task<string?> UploadFileAsync(string userId, byte[] fileData, string fileName)
        {
            try
            {
                SetAuthHeaders();

                // Tạo đường dẫn file: userId/ten_file.pdf
                var safeFileName = Path.GetFileName(fileName);
                var filePath = $"{userId}/{safeFileName}";

                // Endpoint API của Supabase Storage
                var url = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{filePath}";

                // Tạo content upload
                using var content = new ByteArrayContent(fileData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                // Thêm header x-upsert để ghi đè nếu file cũ tồn tại
                if (_httpClient.DefaultRequestHeaders.Contains("x-upsert"))
                    _httpClient.DefaultRequestHeaders.Remove("x-upsert");
                _httpClient.DefaultRequestHeaders.Add("x-upsert", "true");

                _logger.LogInformation($"Đang upload PDF lên Supabase: {filePath}");

                var response = await _httpClient.PostAsync(url, content);

                // Xóa header upsert để tránh ảnh hưởng request sau
                _httpClient.DefaultRequestHeaders.Remove("x-upsert");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Upload thất bại. Status: {response.StatusCode}, Error: {errorBody}");
                    return null;
                }

                // Trả về URL Public để truy cập
                return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{filePath}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ngoại lệ khi upload Supabase");
                return null;
            }
        }
        public string GetPublicUrl(string filePath)
        {
            // API Get Public URL: GET /storage/v1/object/public/{bucket}/{path}
            // Lưu ý: Bucket phải được set là "Public" trên Supabase Dashboard thì link này mới chạy
            return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{filePath}";
        }
        // --- HÀM MỚI 1: Lấy danh sách file trong folder của User (Để tìm tên file) ---
        public async Task<List<string>> ListUserFilesAsync(string userId)
        {
            SetAuthHeaders();

            // API Endpoint: POST /storage/v1/object/list/{bucket}
            var url = $"{_supabaseUrl}/storage/v1/object/list/{_bucketName}";

            // Body request để tìm file trong folder userId
            var body = new
            {
                prefix = $"{userId}/", // Chỉ lấy file trong folder có tên là userId
                limit = 10,            // Lấy 10 file gần nhất
                sortBy = new
                {
                    column = "created_at", // Sắp xếp theo ngày tạo
                    order = "desc"         // Mới nhất lên đầu
                }
            };

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to list files. Status: {Status}, Body: {Body}",
                        response.StatusCode, responseString);
                    return new List<string>();
                }

                // Parse JSON response
                using var doc = JsonDocument.Parse(responseString);
                var files = new List<string>();

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        if (element.TryGetProperty("name", out var nameProp))
                        {
                            var fileName = nameProp.GetString();
                            // Supabase đôi khi trả về folder rỗng hoặc file placeholder, nên cần lọc
                            if (!string.IsNullOrEmpty(fileName) && fileName != ".emptyFolderPlaceholder")
                            {
                                // Trả về Full Path: "userId/ten_file.pdf" để dùng cho hàm Download
                                files.Add($"{userId}/{fileName}");
                            }
                        }
                    }
                }

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception listing files from Supabase");
                return new List<string>();
            }
        }

        // --- HÀM MỚI 2: Tải file từ Supabase về Server ---
        // SỬA ĐỔI: Tạm thời nhận userId để tự tìm file mới nhất
        public async Task<byte[]?> DownloadFileAsync(string userId)
        {
            // 1. Tìm file mới nhất của user
            var files = await ListUserFilesAsync(userId);
            if (files == null || files.Count == 0)
            {
                _logger.LogWarning("No files found for user: {UserId}", userId);
                return null;
            }

            var filePath = files[0]; // File mới nhất (đã sort desc ở ListUserFilesAsync)
            _logger.LogInformation("Downloading latest file for user {UserId}: {FilePath}", userId, filePath);

            SetAuthHeaders();

            // API Endpoint để download file: GET /storage/v1/object/{bucket}/{path}
            var url = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{filePath}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to download file. Status: {Status}, Path: {Path}, Error: {Error}",
                        response.StatusCode, filePath, error);
                    return null;
                }

                // Đọc file thành mảng byte để chuẩn bị gửi cho AI
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception downloading file from Supabase");
                return null;
            }
        }
    }

}
