using IdeaManagement.Models;

namespace IdeaManagement.Repositories;

public interface IIdeaRepository
{
    Task<Idea> CreateIdeaAsync(string content);
    Task<List<Idea>> GetAllIdeasAsync();
    Task<Idea?> GetIdeaByIdAsync(int id);
    Task<Idea> UpdateIdeaAsync(int id, string newContent);
    Task DeleteIdeaAsync(int id);
}
