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
}
