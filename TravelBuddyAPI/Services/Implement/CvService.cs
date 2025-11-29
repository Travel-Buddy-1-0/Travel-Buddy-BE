using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Enum;
using BusinessObject.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class CvService : ICvService
    {
        private readonly ICvRepository _repository;

        public CvService(ICvRepository repository)
        {
            _repository = repository;
        }

        // --- Helpers Serialize/Deserialize ---
        private T? ParseJson<T>(JsonObject? jsonObject)
        {
            if (jsonObject == null) return default;
            // Chuyển JsonObject (DB) thành string rồi parse về Class C#
            return JsonSerializer.Deserialize<T>(jsonObject.ToJsonString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private JsonObject? ToJsonObject<T>(T? obj)
        {
            if (obj == null) return null;
            var jsonString = JsonSerializer.Serialize(obj);
            return JsonNode.Parse(jsonString)?.AsObject();
        }

        // 1. GET TEMPLATES
        public async Task<List<TemplateResponse>> GetAllTemplatesAsync()
        {
            var templates = await _repository.GetTemplatesAsync();
            return templates.Select(t => new TemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                PreviewImage = t.PreviewImage,
                IsPremium = t.IsPremium
            }).ToList();
        }

        // 2. CREATE MANUAL
        public async Task<CvResponse> CreateManualCvAsync(int userId, CreateCvRequest request)
        {
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId);
            if (template == null) throw new Exception("Template not found");

            // Parse default data từ template (đang lưu chuỗi) thành JsonObject
            JsonObject? defaultLayout = !string.IsNullOrEmpty(template.DefaultLayout)
                ? JsonNode.Parse(template.DefaultLayout)?.AsObject() : null;
            JsonObject? defaultData = !string.IsNullOrEmpty(template.DefaultDataJson)
                ? JsonNode.Parse(template.DefaultDataJson)?.AsObject() : null;
            JsonObject? defaultStyle = !string.IsNullOrEmpty(template.StyleSchema)
                ? JsonNode.Parse(template.StyleSchema)?.AsObject() : null;

            var newCv = new Cv
            {
                UserId = userId,
                TemplateId = template.Id,
                Title = request.Title,
                Slug = GenerateSlug(request.Title),
                ThumbnailUrl = template.PreviewImage,
                LayoutConfig = defaultLayout,
                StyleConfig = defaultStyle,
                DataJson = defaultData,
                CreationSource = CreationSource.manual,
                ProcessingStatus = ProcessingStatus.completed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1,
                IsPublished = false
            };

            await _repository.AddCvAsync(newCv);
            return MapToCvResponse(newCv);
        }

        // 3. UPDATE CV
        public async Task<CvResponse> UpdateCvAsync(int cvId, int userId, UpdateCvRequest request)
        {
            var cv = await _repository.GetCvByIdAsync(cvId, userId);
            if (cv == null) throw new KeyNotFoundException("CV not found");

            if (cv.Version != request.Version)
                throw new InvalidOperationException("This CV has been updated elsewhere. Please reload.");

            if (!string.IsNullOrEmpty(request.Title))
            {
                cv.Title = request.Title;
                cv.Slug = GenerateSlug(request.Title);
            }

            // Map strict models -> JsonObject
            if (request.LayoutConfig != null) cv.LayoutConfig = ToJsonObject(request.LayoutConfig);
            if (request.StyleConfig != null) cv.StyleConfig = ToJsonObject(request.StyleConfig);
            if (request.DataJson != null) cv.DataJson = ToJsonObject(request.DataJson);

            cv.Version = request.Version + 1;
            cv.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateCvAsync(cv);
            return MapToCvResponse(cv);
        }

        // 4. GET MY CVS
        public async Task<List<CvListItemResponse>> GetMyCvsAsync(int userId)
        {
            var cvs = await _repository.GetCvsByUserIdAsync(userId);
            return cvs.Select(c => new CvListItemResponse
            {
                Id = c.Id,
                Title = c.Title,
                ThumbnailUrl = c.ThumbnailUrl,
                IsPublished = c.IsPublished,
                ProcessingStatus = c.ProcessingStatus.ToString(),
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }

        // 5. GET DETAIL
        public async Task<CvResponse> GetCvDetailAsync(int cvId, int userId)
        {
            var cv = await _repository.GetCvByIdAsync(cvId, userId);
            if (cv == null) throw new KeyNotFoundException("CV not found");
            return MapToCvResponse(cv);
        }

        // 6. DELETE
        public async Task DeleteCvAsync(int cvId, int userId)
        {
            var cv = await _repository.GetCvByIdAsync(cvId, userId);
            if (cv == null) throw new KeyNotFoundException("CV not found");
            await _repository.DeleteCvAsync(cv);
        }

        // 7. UPLOAD
        public async Task<UploadCvResponse> UploadCvAsync(int userId, UploadCvRequest request)
        {
            // TODO: Xử lý file upload thật ở đây
            string fakeRawData = "Extracted text from PDF...";

            var newCv = new Cv
            {
                UserId = userId,
                TemplateId = request.TemplateId,
                Title = request.File.FileName,
                CreationSource = CreationSource.file_import,
                ProcessingStatus = ProcessingStatus.pending,
                CvRawData = fakeRawData,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _repository.AddCvAsync(newCv);
            return new UploadCvResponse { Id = newCv.Id };
        }

        // -- PRIVATE HELPERS --
        private string GenerateSlug(string title) =>
            title.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString().Substring(0, 4);

        private CvResponse MapToCvResponse(Cv cv)
        {
            return new CvResponse
            {
                Id = cv.Id,
                Title = cv.Title,
                Slug = cv.Slug,
                ThumbnailUrl = cv.ThumbnailUrl,
                LayoutConfig = ParseJson<CvLayoutConfig>(cv.LayoutConfig),
                StyleConfig = ParseJson<CvStyleConfig>(cv.StyleConfig),
                DataJson = ParseJson<CvData>(cv.DataJson),
                CreationSource = cv.CreationSource.ToString(),
                ProcessingStatus = cv.ProcessingStatus.ToString(),
                CreatedAt = cv.CreatedAt,
                UpdatedAt = cv.UpdatedAt,
                Version = cv.Version
            };
        }
    }
}
