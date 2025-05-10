using IdeaManagement.Data;
using IdeaManagement.Models;
using IdeaManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace IdeaManagement.Tests;

public class IdeaRepositoryTests
{
    private readonly Mock<IdeaDbContext> _contextMock;
    private readonly Mock<DbSet<Idea>> _dbSetMock;
    private readonly IdeaRepository _repository;

    public IdeaRepositoryTests()
    {
        _contextMock = new Mock<IdeaDbContext>();
        _dbSetMock = new Mock<DbSet<Idea>>();
        _contextMock.Setup(x => x.Ideas).Returns(_dbSetMock.Object);
        _repository = new IdeaRepository(_contextMock.Object);
    }

    [Fact]
    public async Task CreateIdea_ShouldAddIdeaToDatabase()
    {
        // Arrange
        var content = "Test Idea";

        // Act
        var result = await _repository.CreateIdeaAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(content, result.Content);
        _dbSetMock.Verify(x => x.Add(It.IsAny<Idea>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetIdeaById_ShouldReturnIdea_WhenIdeaExists()
    {
        // Arrange
        var testIdea = new Idea { Id = 1, Content = "Test" };
        _dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync(testIdea);

        // Act
        var result = await _repository.GetIdeaByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testIdea.Id, result.Id);
        Assert.Equal(testIdea.Content, result.Content);
    }

    [Fact]
    public async Task DeleteIdea_ShouldThrowException_WhenIdeaDoesNotExist()
    {
        // Arrange
        _dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync((Idea?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.DeleteIdeaAsync(1));
    }

    [Fact]
    public async Task UpdateIdea_ShouldUpdateContent_WhenIdeaExists()
    {
        // Arrange
        var testIdea = new Idea { Id = 1, Content = "Original" };
        _dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync(testIdea);

        // Act
        var result = await _repository.UpdateIdeaAsync(1, "Updated");

        // Assert
        Assert.Equal("Updated", result.Content);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
