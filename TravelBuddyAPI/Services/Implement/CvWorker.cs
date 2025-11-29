using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class CvWorker : BackgroundService
    {
        private readonly JobQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CvWorker> _logger;

        public CvWorker(JobQueueService queue, IServiceScopeFactory scopeFactory, ILogger<CvWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CvWorker is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Chờ và lấy userId từ hàng đợi
                    var userId = await _queue.DequeueAsync(stoppingToken);

                    _logger.LogInformation("Processing job for User: {UserId}", userId);

                    // 2. Tạo scope mới vì Worker là Singleton, còn Services thường là Scoped
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var supabaseService = scope.ServiceProvider.GetRequiredService<SupabaseService>();
                        var geminiService = scope.ServiceProvider.GetRequiredService<GeminiService>();

                        // 3. Tải file mới nhất từ Supabase
                        var fileBytes = await supabaseService.DownloadFileAsync(userId);

                        if (fileBytes != null)
                        {
                            // Lấy tên file để xác định extension (Tạm thời lấy file đầu tiên trong list để lấy tên)
                            // Ở bước này bạn có thể tối ưu hàm DownloadFileAsync để trả về cả tên file
                            // Nhưng để test nhanh, mình giả định là PDF hoặc lấy từ list
                            var files = await supabaseService.ListUserFilesAsync(userId);
                            string fileName = files.FirstOrDefault() ?? "unknown.pdf";

                            // 4. Gửi sang Gemini Scan
                            _logger.LogInformation("Sending file {FileName} to Gemini...", fileName);
                            //var rawText = await geminiService(fileBytes, fileName);

                            //if (!string.IsNullOrEmpty(rawText))
                            //{
                            //    _logger.LogInformation("=== KẾT QUẢ AI SCAN CHO USER {UserId} ===", userId);
                            //    Debug.WriteLine(rawText);
                            //    // TODO: Sau này sẽ lưu rawText vào DB tại đây
                            //}
                            //else
                            //{
                            //    _logger.LogWarning("Gemini returned empty text.");
                            //}
                        }
                        else
                        {
                            _logger.LogWarning("Could not download file for User: {UserId}", userId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing job.");
                }
            }
        }
    }
}
