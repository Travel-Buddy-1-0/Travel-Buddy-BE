using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICvService
    {
        Task<List<TemplateResponse>> GetAllTemplatesAsync();
        Task<CvResponse> CreateManualCvAsync(int userId, CreateCvRequest request);
        Task<UploadCvResponse> UploadCvAsync(int userId, UploadCvRequest request);
        Task<List<CvListItemResponse>> GetMyCvsAsync(int userId);
        Task<CvResponse> GetCvDetailAsync(int cvId, int userId);
        Task<CvResponse> UpdateCvAsync(int cvId, int userId, UpdateCvRequest request);
        Task DeleteCvAsync(int cvId, int userId);
    }
}
