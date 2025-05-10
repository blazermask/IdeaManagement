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
        return await _context.Ideas.OrderBy(i => i.Id).ToListAsync();
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

        // Create a new idea with the new ID
        var newIdea = new Idea
        {
            Id = newId,
            Content = idea.Content,
            CreatedDate = idea.CreatedDate,
            ModifiedDate = idea.ModifiedDate
        };

        // Remove the old idea and add the new one
        _context.Ideas.Remove(idea);
        _context.Ideas.Add(newIdea);

        await _context.SaveChangesAsync();
    }

    public async Task ReorderIdsAsync()
    {
        var ideas = await _context.Ideas.OrderBy(i => i.CreatedDate).ToListAsync();
        var tempList = ideas.Select(i => new
        {
            OldId = i.Id,
            NewId = -i.Id, // Temporary negative ID to avoid conflicts
            Idea = i
        }).ToList();

        // First pass: set temporary negative IDs
        foreach (var item in tempList)
        {
            var idea = await _context.Ideas.FindAsync(item.OldId);
            if (idea != null)
            {
                _context.Ideas.Remove(idea);
                _context.Ideas.Add(new Idea
                {
                    Id = item.NewId,
                    Content = idea.Content,
                    CreatedDate = idea.CreatedDate,
                    ModifiedDate = idea.ModifiedDate
                });
            }
        }
        await _context.SaveChangesAsync();

        // Second pass: set final IDs
        for (int i = 0; i < tempList.Count; i++)
        {
            var idea = await _context.Ideas.FindAsync(tempList[i].NewId);
            if (idea != null)
            {
                _context.Ideas.Remove(idea);
                _context.Ideas.Add(new Idea
                {
                    Id = i + 1,
                    Content = idea.Content,
                    CreatedDate = idea.CreatedDate,
                    ModifiedDate = idea.ModifiedDate
                });
            }
        }
        await _context.SaveChangesAsync();
    }
}