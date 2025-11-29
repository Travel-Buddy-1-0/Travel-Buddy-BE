using Microsoft.Extensions.Logging;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class PuppeteerSharpPDFServices : IAsyncDisposable
    {
        private IBrowser? _browser;
        private readonly ILogger<PuppeteerSharpPDFServices> _logger;

        public PuppeteerSharpPDFServices(ILogger<PuppeteerSharpPDFServices> logger)
        {
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            if (_browser == null || _browser.IsClosed)
            {
                _logger.LogInformation("🚀 Đang khởi tạo Puppeteer (Chrome)...");

                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-gpu",
                        // Thêm flag này để font render đẹp hơn trên Linux/Docker
                        "--font-render-hinting=none"
                    }
                });
                _logger.LogInformation("✅ Puppeteer đã sẵn sàng!");
            }
        }

        public async Task<byte[]> GeneratePdfAsync(string htmlContent)
        {
            // 1. Bắt đầu đo giờ
            var stopwatch = Stopwatch.StartNew();

            await InitializeAsync();

            if (_browser == null) throw new Exception("Không thể khởi tạo Browser");

            using var page = await _browser.NewPageAsync();

            try
            {
                // --- FIX LỖI 2 CỘT THÀNH 1 CỘT ---

                // 1. Set Viewport thật to để Tailwind nhận diện là Desktop
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1920,
                    Height = 1080
                });

                // 2. Ép trình duyệt render kiểu "Màn hình" thay vì kiểu "Máy in"
                // Giúp giữ nguyên màu nền và layout grid/flex
                await page.EmulateMediaTypeAsync(MediaType.Screen);

                // --------------------------------

                // Set nội dung HTML
                await page.SetContentAsync(htmlContent, new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                });

                var pdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "0",
                        Bottom = "0",
                        Left = "0",
                        Right = "0"
                    }
                };

                var pdfBytes = await page.PdfDataAsync(pdfOptions);

                // 2. Dừng đo giờ và Log
                stopwatch.Stop();
                _logger.LogInformation($"⏱️ [PERFORMANCE] Tạo PDF mất: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} giây)");

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi render PDF");
                throw;
            }
            finally
            {
                await page.CloseAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
        }
    }
}
