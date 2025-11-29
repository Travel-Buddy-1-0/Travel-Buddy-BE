using BusinessObject.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Services.Implement
{
    public class QuestPdfService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public QuestPdfService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateCvFromDataAsync(CvGenerationRequest data)
        {
            // Tải Avatar
            byte[]? avatarBytes = null;
            if (data.DataJson?.Profile != null && !string.IsNullOrEmpty(data.DataJson.Profile.Avatar))
            {
                avatarBytes = await DownloadImageAsync(data.DataJson.Profile.Avatar);
            }

            // Lấy Global Styles
            var globalStyles = data.StyleJson.Global ?? new Dictionary<string, string>();
            var bgColor = ParseColor(GetStyleValue(globalStyles, "background", "#ffffff"));
            var fontColor = ParseColor(GetStyleValue(globalStyles, "color", "#222222"));
            var fontFamily = GetStyleValue(globalStyles, "fontFamily", "Arial").Split(',')[0].Trim();

            // Padding & Font Size
            var padding = ParseSize(GetStyleValue(globalStyles, "padding", "20px"));
            var baseFontSize = ParseSize(GetStyleValue(globalStyles, "fontSize", "10px"));

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(padding);
                    page.PageColor(bgColor);
                    page.DefaultTextStyle(x => x.FontFamily(fontFamily).FontSize(baseFontSize).FontColor(fontColor));

                    page.Content().Column(col =>
                    {
                        // XỬ LÝ LAYOUT DYNAMIC
                        if (data.LayoutJson.Layout.Type == "two-column")
                        {
                            col.Item().Row(row =>
                            {
                                // CỘT TRÁI
                                row.RelativeItem(3.5f)
                                   .BorderRight(1).BorderColor(Colors.Grey.Lighten2)
                                   .PaddingRight(15)
                                   .Column(leftCol =>
                                   {
                                       var sections = data.LayoutJson.Layout.Left ?? new List<string>();
                                       foreach (var section in sections)
                                           RenderSection(leftCol, section, data.DataJson, data.StyleJson, avatarBytes);
                                   });

                                // CỘT PHẢI
                                row.RelativeItem(6.5f)
                                   .PaddingLeft(15)
                                   .Column(rightCol =>
                                   {
                                       var sections = data.LayoutJson.Layout.Right ?? new List<string>();
                                       foreach (var section in sections)
                                           RenderSection(rightCol, section, data.DataJson, data.StyleJson, avatarBytes);
                                   });
                            });
                        }
                        else // One Column
                        {
                            var sections = data.LayoutJson.SectionsOrder ?? new List<string>();
                            foreach (var section in sections)
                                RenderSection(col, section, data.DataJson, data.StyleJson, avatarBytes);
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        // --- HELPER METHODS ---

        private string GetStyleValue(Dictionary<string, string>? styles, string key, string defaultValue)
        {
            if (styles != null && styles.TryGetValue(key, out var value)) return value;
            return defaultValue;
        }

        private float ParseSize(string sizeStr)
        {
            if (string.IsNullOrEmpty(sizeStr)) return 0;
            var numPart = new string(sizeStr.Where(c => char.IsDigit(c) || c == '.').ToArray());

            if (float.TryParse(numPart, out var result))
            {
                // Quy đổi PX sang Point cho kích thước chuẩn hơn (96dpi vs 72dpi)
                if (sizeStr.EndsWith("px")) return result * 0.75f;
                return result;
            }
            return 10;
        }

        private Color ParseColor(string hex)
        {
            try
            {
                // SỬA LỖI 1: Dùng Color.FromHex thay vì ParseHex
                return Color.FromHex(hex);
            }
            catch
            {
                return Colors.Black;
            }
        }

        private async Task<byte[]> DownloadImageAsync(string url)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                return await client.GetByteArrayAsync(url);
            }
            catch
            {
                return null;
            }
        }

        // --- RENDER LOGIC ---

        private void RenderSection(ColumnDescriptor col, string sectionName, DataJson data, StyleJson styles, byte[]? avatarBytes)
        {
            if (data == null) return;

            var contentStyles = styles.Section?.Content ?? new Dictionary<string, string>();
            var marginBottom = ParseSize(GetStyleValue(contentStyles, "marginBottom", "10px"));
            var textColor = ParseColor(GetStyleValue(contentStyles, "color", "#444444"));

            col.Item().PaddingBottom(marginBottom).Column(sectionCol =>
            {
                switch (sectionName)
                {
                    case "profile":
                        if (data.Profile != null)
                            RenderProfile(sectionCol, data.Profile, styles.Avatar, avatarBytes, styles.Section?.Title);
                        break;

                    case "summary":
                        DrawTitle(sectionCol, "Professional Summary", styles.Section?.Title);
                        sectionCol.Item().Text(data.Summary).FontColor(textColor);
                        break;

                    case "experience":
                        DrawTitle(sectionCol, "Work Experience", styles.Section?.Title);
                        if (data.Experience != null)
                        {
                            foreach (var exp in data.Experience)
                            {
                                sectionCol.Item().PaddingBottom(10).Column(c =>
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text(exp.Position).Bold().FontColor(textColor);
                                        r.AutoItem().Text(exp.Time).FontSize(9).FontColor(Colors.Blue.Medium);
                                    });
                                    c.Item().Text(exp.Company).Italic().FontSize(9).FontColor(Colors.Grey.Darken1);
                                    c.Item().Text(exp.Desc).FontSize(9).FontColor(textColor);
                                });
                            }
                        }
                        break;

                    case "projects":
                        DrawTitle(sectionCol, "Projects", styles.Section?.Title);
                        if (data.Projects != null)
                        {
                            foreach (var proj in data.Projects)
                            {
                                sectionCol.Item().PaddingBottom(10).Column(c =>
                                {
                                    c.Item().Text(proj.Name).Bold().FontColor(textColor);
                                    c.Item().PaddingBottom(2).Inlined(inlined =>
                                    {
                                        foreach (var tech in proj.Tech)
                                        {
                                            inlined.Item().PaddingRight(5).Text($"[{tech}]").FontSize(8).FontColor(Colors.Grey.Darken1);
                                        }
                                    });
                                    c.Item().Text(proj.Desc).FontSize(9).FontColor(textColor);
                                });
                            }
                        }
                        break;

                    case "skills":
                        DrawTitle(sectionCol, "Skills", styles.Section?.Title);
                        var skillStyles = styles.Components != null && styles.Components.ContainsKey("skill-tag")
                                            ? styles.Components["skill-tag"] : new Dictionary<string, string>();
                        var skillBg = ParseColor(GetStyleValue(skillStyles, "background", "#e8f2ff"));
                        var skillColor = ParseColor(GetStyleValue(skillStyles, "color", "#0052cc"));

                        if (data.Skills != null)
                        {
                            sectionCol.Item().Inlined(inlined =>
                            {
                                foreach (var skill in data.Skills)
                                {
                                    inlined.Item().Background(skillBg).Padding(2).PaddingHorizontal(5).Text(skill).FontSize(9).FontColor(skillColor);
                                    inlined.Item().Width(5);
                                }
                            });
                        }
                        break;

                    case "education":
                        DrawTitle(sectionCol, "Education", styles.Section?.Title);
                        if (data.Education != null)
                        {
                            foreach (var edu in data.Education)
                            {
                                sectionCol.Item().PaddingBottom(5).Column(c =>
                                {
                                    c.Item().Text(edu.School).Bold().FontColor(textColor);
                                    c.Item().Text(edu.Degree).FontSize(9).FontColor(textColor);
                                    c.Item().Text(edu.Year).FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                                });
                            }
                        }
                        break;

                    case "languages":
                        if (data.Languages != null && data.Languages.Any())
                        {
                            DrawTitle(sectionCol, "Languages", styles.Section?.Title);
                            foreach (var lang in data.Languages)
                            {
                                sectionCol.Item().Text($"• {lang}").FontColor(textColor);
                            }
                        }
                        break;

                    case "interests":
                        if (data.Interests != null && data.Interests.Any())
                        {
                            DrawTitle(sectionCol, "Interests", styles.Section?.Title);
                            foreach (var interest in data.Interests)
                            {
                                sectionCol.Item().Text($"• {interest}").FontColor(textColor);
                            }
                        }
                        break;
                }
            });
        }

        private void RenderProfile(ColumnDescriptor col, ProfileData profile, Dictionary<string, string>? avatarStyles, byte[]? avatarBytes, Dictionary<string, string>? titleStyles)
        {
            var nameColor = ParseColor(GetStyleValue(titleStyles, "color", "#0066ff"));

            if (avatarBytes != null)
            {
                var width = ParseSize(GetStyleValue(avatarStyles, "width", "100px"));

                // CẬP NHẬT: Bỏ ClipToCircle() gây lỗi, chỉ hiển thị ảnh vuông
                col.Item().AlignCenter()
                   .Width(width)
                   .Height(width)
                   .Image(avatarBytes);
            }

            col.Item().PaddingTop(10).AlignCenter().Text(profile.Name).Bold().FontSize(18).FontColor(nameColor);
            col.Item().AlignCenter().Text(profile.Email).FontSize(9);
            col.Item().AlignCenter().Text(profile.Phone).FontSize(9);
        }

        private void DrawTitle(ColumnDescriptor col, string text, Dictionary<string, string>? titleStyles)
        {
            var color = ParseColor(GetStyleValue(titleStyles, "color", "#0066ff"));
            var fontSize = ParseSize(GetStyleValue(titleStyles, "fontSize", "14px"));
            var mb = ParseSize(GetStyleValue(titleStyles, "marginBottom", "5px"));
            var transform = GetStyleValue(titleStyles, "textTransform", "none");
            var finalText = transform == "uppercase" ? text.ToUpper() : text;

            col.Item().PaddingBottom(mb)
                .BorderBottom(2).BorderColor(color)
                .Text(finalText).FontSize(fontSize).Bold().FontColor(color);
        }
    }
}
