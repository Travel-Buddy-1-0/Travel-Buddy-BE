using BusinessObject.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.DTOs
{
    // RESPONSE: TEMPLATE
    public class TemplateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [JsonPropertyName("preview_image")]
        public string? PreviewImage { get; set; }
        [JsonPropertyName("is_premium")]
        public bool? IsPremium { get; set; }
    }

    // REQUEST: CREATE
    public class CreateCvRequest
    {
        public int TemplateId { get; set; }
        public string Title { get; set; } = null!;
    }

    // REQUEST: UPDATE
    public class UpdateCvRequest
    {
        public string? Title { get; set; }
        [JsonPropertyName("layout_config")]
        public CvLayoutConfig? LayoutConfig { get; set; }
        [JsonPropertyName("style_config")]
        public CvStyleConfig? StyleConfig { get; set; }
        [JsonPropertyName("data_json")]
        public CvData? DataJson { get; set; }
        public decimal Version { get; set; }
    }

    // REQUEST: UPLOAD
    public class UploadCvRequest
    {
        public IFormFile File { get; set; } = null!;
        public int TemplateId { get; set; }
    }

    // RESPONSE: CV DETAIL (FULL)
    public class CvResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        [JsonPropertyName("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("layout_config")]
        public CvLayoutConfig? LayoutConfig { get; set; }

        [JsonPropertyName("style_config")]
        public CvStyleConfig? StyleConfig { get; set; }

        [JsonPropertyName("data_json")]
        public CvData? DataJson { get; set; }

        [JsonPropertyName("creation_source")]
        public string CreationSource { get; set; } = null!;

        [JsonPropertyName("processing_status")]
        public string ProcessingStatus { get; set; } = null!;

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        public decimal? Version { get; set; }
    }

    // RESPONSE: CV LIST ITEM (SHORT)
    public class CvListItemResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [JsonPropertyName("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }
        [JsonPropertyName("is_published")]
        public bool? IsPublished { get; set; }
        [JsonPropertyName("processing_status")]
        public string ProcessingStatus { get; set; } = null!;
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    // RESPONSE: UPLOAD RESULT
    public class UploadCvResponse
    {
        public int Id { get; set; }
    }
}
