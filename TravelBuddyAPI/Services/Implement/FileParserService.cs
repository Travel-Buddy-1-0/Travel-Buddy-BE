using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Services.Implement
{
    public class FileParserService
    {
        private readonly ILogger<FileParserService> _logger;

        public FileParserService(ILogger<FileParserService> logger)
        {
            _logger = logger;
        }

        // --- HÀM FORMAT ĐƠN GIẢN: CHUYỂN HẾT VỀ CHỮ THƯỜNG ---
        private string FormatText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Tách dòng để xử lý từng dòng một (giữ cấu trúc xuống dòng)
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0) continue;

                // CHỈ CẦN DÒNG NÀY: Chuyển toàn bộ về chữ thường
                sb.AppendLine(trimmed);
            }

            return sb.ToString().Trim();
        }

        // --- 1. XỬ LÝ PDF ---
        public string ParsePdf(Stream fileStream)
        {
            var sb = new StringBuilder();

            try
            {
                if (fileStream.Position > 0) fileStream.Position = 0;

                using (var pdf = PdfDocument.Open(fileStream))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        string rawText = ContentOrderTextExtractor.GetText(page, true);

                        // Format chữ thường
                        string cleanText = FormatText(rawText);

                        if (!string.IsNullOrEmpty(cleanText))
                        {
                            sb.AppendLine(cleanText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }

            string finalResult = sb.ToString();
            _logger.LogInformation(finalResult); // In ra log kết quả cuối cùng

            return finalResult;
        }

        // --- 2. XỬ LÝ DOCX ---
        public string ParseDocx(Stream fileStream)
        {
            var sb = new StringBuilder();

            try
            {
                if (fileStream.Position > 0) fileStream.Position = 0;

                using (var wordDoc = WordprocessingDocument.Open(fileStream, false))
                {
                    var body = wordDoc.MainDocumentPart?.Document.Body;
                    if (body != null)
                    {
                        foreach (var para in body.Elements<Paragraph>())
                        {
                            // Format chữ thường từng đoạn
                            string cleanText = FormatText(para.InnerText);

                            if (!string.IsNullOrEmpty(cleanText))
                            {
                                sb.AppendLine(cleanText);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }

            string finalResult = sb.ToString();
            _logger.LogInformation(finalResult); // In ra log kết quả cuối cùng

            return finalResult;
        }
    }
}
