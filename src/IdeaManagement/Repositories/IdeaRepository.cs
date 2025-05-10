using IdeaManagement.Data;
using IdeaManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeaManagement.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly IdeaDbContext _context;

    public IdeaRepository(IdeaDbContext context)
    {
        _context = context;
    }

    public async Task<Idea> CreateIdeaAsync(string content)
    {
        var idea = new Idea
        {
            Content = content,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _context.Ideas.Add(idea);
        await _context.SaveChangesAsync();
        return idea;
    }

    public async Task<Idea> CreateIdeaAsync(int id, string content)
    {
        var idea = new Idea
        {
            Id = id,
            Content = content,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _context.Ideas.Add(idea);
        await _context.SaveChangesAsync();
        return idea;
    }

    public async Task<List<Idea>> GetAllIdeasAsync()
    {
        return await _context.Ideas.ToListAsync();
    }

    public async Task<Idea?> GetIdeaByIdAsync(int id)
    {
        return await _context.Ideas.FindAsync(id);
    }

    public async Task<Idea> UpdateIdeaAsync(int id, string newContent)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) throw new KeyNotFoundException($"Idea with ID {id} not found");

        idea.Content = newContent;
        idea.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return idea;
    }

    public async Task DeleteIdeaAsync(int id)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) throw new KeyNotFoundException($"Idea with ID {id} not found");

        _context.Ideas.Remove(idea);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeIdeaIdAsync(int oldId, int newId)
    {
        var idea = await _context.Ideas.FindAsync(oldId);
        if (idea == null) throw new KeyNotFoundException($"Idea with ID {oldId} not found");

        var existingIdea = await _context.Ideas.FindAsync(newId);
        if (existingIdea != null) throw new InvalidOperationException($"Idea with ID {newId} already exists");

        idea.Id = newId;
        await _context.SaveChangesAsync();
    }

    public async Task ReorderIdsAsync()
    {
        var ideas = await _context.Ideas.OrderBy(i => i.CreatedDate).ToListAsync();
        for (int i = 0; i < ideas.Count; i++)
        {
            ideas[i].Id = i + 1;
        }
        await _context.SaveChangesAsync();
    }
}
