using BusinessObject.Entities;

namespace Repositories.Interfaces
{
    public interface ICvRepository
    {
        Task<List<Template>> GetTemplatesAsync();
        Task<Template?> GetTemplateByIdAsync(int id);
        Task<List<Cv>> GetCvsByUserIdAsync(int userId);
        Task<Cv?> GetCvByIdAsync(int cvId, int userId);
        Task<Cv> AddCvAsync(Cv cv);
        Task UpdateCvAsync(Cv cv);
        Task DeleteCvAsync(Cv cv);
    }
}
