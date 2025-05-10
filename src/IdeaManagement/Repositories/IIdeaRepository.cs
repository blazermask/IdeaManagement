// IIdeaRepository.cs
using IdeaManagement.Models;

namespace IdeaManagement.Repositories;

public interface IIdeaRepository
{
    Task<Idea> CreateIdeaAsync(string content);
    Task<Idea> CreateIdeaAsync(int id, string content);
    Task<List<Idea>> GetAllIdeasAsync();
    Task<Idea?> GetIdeaByIdAsync(int id);
    Task<Idea> UpdateIdeaAsync(int id, string newContent);
    Task DeleteIdeaAsync(int id);
    Task ChangeIdeaIdAsync(int oldId, int newId);
    Task ReorderIdsAsync();
    Task RemoveAllIdeasAsync();

}
