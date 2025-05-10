using Microsoft.EntityFrameworkCore;
using IdeaManagement.Data;

namespace IdeaManagement.Tests;

public static class TestHelper
{
    private static string? _connectionString;

    public static void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static IdeaDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdeaDbContext>();

        if (string.IsNullOrEmpty(_connectionString))
        {
            // Use in-memory database if no connection string is provided
            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        }
        else
        {
            // Use MySQL if connection string is provided
            optionsBuilder.UseMySql(
                _connectionString,
                new MySqlServerVersion(new Version(8, 0, 0))
            );
        }

        return new IdeaDbContext(optionsBuilder.Options);
    }
}
