using IdeaManagement.Data;
using IdeaManagement.Models;
using IdeaManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdeaManagement.Tests;

public class IdeaRepositoryTests : IDisposable
{
    private readonly IdeaDbContext _context;
    private readonly IdeaRepository _repository;

    public IdeaRepositoryTests()
    {
        _context = TestHelper.CreateDbContext();
        _repository = new IdeaRepository(_context);

        // Ensure database is clean before each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateIdea_ShouldAssignLowestAvailableId()
    {
        // Arrange
        var content = "Test Idea";

        // Act
        var idea = await _repository.CreateIdeaAsync(content);

        // Assert
        Assert.Equal(1, idea.Id);
        Assert.Equal(content, idea.Content);
        Assert.NotEqual(default, idea.CreatedDate);
        Assert.NotEqual(default, idea.ModifiedDate);
    }

    [Fact]
    public async Task CreateIdea_WithGaps_ShouldUseLowestAvailableId()
    {
        // Arrange
        await _repository.CreateIdeaAsync(1, "First");
        await _repository.CreateIdeaAsync(3, "Third");

        // Act
        var idea = await _repository.CreateIdeaAsync("New Idea");

        // Assert
        Assert.Equal(2, idea.Id);
    }

    [Fact]
    public async Task GetAllIdeas_ShouldReturnOrderedByIdList()
    {
        // Arrange
        await _repository.CreateIdeaAsync(2, "Second");
        await _repository.CreateIdeaAsync(1, "First");
        await _repository.CreateIdeaAsync(3, "Third");

        // Act
        var ideas = await _repository.GetAllIdeasAsync();

        // Assert
        Assert.Equal(3, ideas.Count);
        Assert.Equal(1, ideas[0].Id);
        Assert.Equal(2, ideas[1].Id);
        Assert.Equal(3, ideas[2].Id);
    }

    [Fact]
    public async Task GetAllIdeas_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var ideas = await _repository.GetAllIdeasAsync();

        // Assert
        Assert.Empty(ideas);
    }

    [Fact]
    public async Task GetIdeaById_ExistingId_ShouldReturnIdea()
    {
        // Arrange
        var created = await _repository.CreateIdeaAsync("Test Idea");

        // Act
        var retrieved = await _repository.GetIdeaByIdAsync(created.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Content, retrieved.Content);
    }

    [Fact]
    public async Task GetIdeaById_NonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetIdeaByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateIdea_ShouldModifyContentAndUpdateTimestamp()
    {
        // Arrange
        var idea = await _repository.CreateIdeaAsync("Original");
        var originalModified = idea.ModifiedDate;
        await Task.Delay(10); // Ensure time difference

        // Act
        var updated = await _repository.UpdateIdeaAsync(idea.Id, "Updated");

        // Assert
        Assert.Equal("Updated", updated.Content);
        Assert.True(updated.ModifiedDate > originalModified);
        Assert.Equal(idea.CreatedDate, updated.CreatedDate);
    }

    [Fact]
    public async Task UpdateIdea_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _repository.UpdateIdeaAsync(999, "New Content"));
    }

    [Fact]
    public async Task DeleteIdea_ShouldRemoveIdeaFromDatabase()
    {
        // Arrange
        var idea = await _repository.CreateIdeaAsync("To Delete");

        // Act
        await _repository.DeleteIdeaAsync(idea.Id);

        // Assert
        var result = await _repository.GetIdeaByIdAsync(idea.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteIdea_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _repository.DeleteIdeaAsync(999));
    }

    [Fact]
    public async Task ChangeIdeaId_ShouldUpdateIdAndMaintainData()
    {
        // Arrange
        var idea = await _repository.CreateIdeaAsync("Test");
        var originalContent = idea.Content;
        var originalCreated = idea.CreatedDate;
        var originalModified = idea.ModifiedDate;

        // Act
        await _repository.ChangeIdeaIdAsync(idea.Id, 100);

        // Assert
        var updated = await _repository.GetIdeaByIdAsync(100);
        Assert.NotNull(updated);
        Assert.Equal(originalContent, updated.Content);
        Assert.Equal(originalCreated, updated.CreatedDate);
        Assert.Equal(originalModified, updated.ModifiedDate);

        // Verify old ID no longer exists
        var oldIdea = await _repository.GetIdeaByIdAsync(idea.Id);
        Assert.Null(oldIdea);
    }

    [Fact]
    public async Task ChangeIdeaId_ToExistingId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var idea1 = await _repository.CreateIdeaAsync("First");
        var idea2 = await _repository.CreateIdeaAsync("Second");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.ChangeIdeaIdAsync(idea1.Id, idea2.Id));
    }

    [Fact]
    public async Task ChangeIdeaId_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _repository.ChangeIdeaIdAsync(999, 1));
    }

    [Fact]
    public async Task ReorderIds_ShouldCreateSequentialIds()
    {
        // Arrange
        await _repository.CreateIdeaAsync(1, "First");
        await _repository.CreateIdeaAsync(3, "Third");
        await _repository.CreateIdeaAsync(5, "Fifth");

        // Act
        await _repository.ReorderIdsAsync();

        // Assert
        var ideas = await _repository.GetAllIdeasAsync();
        Assert.Equal(3, ideas.Count);
        Assert.Equal(1, ideas[0].Id);
        Assert.Equal(2, ideas[1].Id);
        Assert.Equal(3, ideas[2].Id);
    }

    [Fact]
    public async Task ReorderIds_EmptyDatabase_ShouldNotThrowException()
    {
        // Act & Assert
        await _repository.ReorderIdsAsync();
        var ideas = await _repository.GetAllIdeasAsync();
        Assert.Empty(ideas);
    }

    [Fact]
    public async Task RemoveAllIdeas_ShouldDeleteAllIdeas()
    {
        // Arrange
        await _repository.CreateIdeaAsync("First");
        await _repository.CreateIdeaAsync("Second");
        await _repository.CreateIdeaAsync("Third");

        // Act
        await _repository.RemoveAllIdeasAsync();

        // Assert
        var ideas = await _repository.GetAllIdeasAsync();
        Assert.Empty(ideas);
    }

    [Fact]
    public async Task RemoveAllIdeas_EmptyDatabase_ShouldNotThrowException()
    {
        // Act & Assert
        await _repository.RemoveAllIdeasAsync();
        var ideas = await _repository.GetAllIdeasAsync();
        Assert.Empty(ideas);
    }

    [Fact]
    public async Task CreateIdea_WithExistingId_ShouldCreateIdeaWithSpecifiedId()
    {
        // Arrange
        const int specificId = 42;
        const string content = "Specific ID Idea";

        // Act
        var idea = await _repository.CreateIdeaAsync(specificId, content);

        // Assert
        Assert.Equal(specificId, idea.Id);
        Assert.Equal(content, idea.Content);
        Assert.NotEqual(default, idea.CreatedDate);
        Assert.NotEqual(default, idea.ModifiedDate);
    }

    [Fact]
    public async Task CreateIdea_WithExistingId_ShouldThrowException()
    {
        // Arrange
        await _repository.CreateIdeaAsync(1, "First");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateIdeaAsync(1, "Duplicate"));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
