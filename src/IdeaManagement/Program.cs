using IdeaManagement.Data;
using IdeaManagement.Models;
using IdeaManagement.Repositories;
using IdeaManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var credentialManager = new DatabaseCredentialManager();
DatabaseCredentials? credentials = null;

// Try to load saved credentials
var savedCredentials = credentialManager.LoadCredentials();
if (savedCredentials != null)
{
    Console.WriteLine("Found saved credentials. Attempting automatic login...");
    credentials = savedCredentials;
}

// If no saved credentials or user wants to login manually
if (credentials == null)
{
    credentials = PromptForCredentials();
}

var services = new ServiceCollection();

try
{
    // Configure services with the provided credentials
    services.AddDbContext<IdeaDbContext>(options =>
        options.UseMySql(
            credentials.GetConnectionString(),
            new MySqlServerVersion(new Version(8, 0, 0))
        ));

    services.AddScoped<IIdeaRepository, IdeaRepository>();

    var serviceProvider = services.BuildServiceProvider();

    // Test the connection
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<IdeaDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    // If connection successful and credentials weren't saved, ask to save
    if (savedCredentials == null)
    {
        Console.Write("Would you like to save these credentials? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            credentialManager.SaveCredentials(credentials);
            Console.WriteLine("Credentials saved successfully.");
        }
    }

    var repository = serviceProvider.GetRequiredService<IIdeaRepository>();

    while (true)
    {
        Console.WriteLine("\nIdea Management System");
        Console.WriteLine("1. Create new idea");
        Console.WriteLine("2. View all ideas");
        Console.WriteLine("3. Update idea");
        Console.WriteLine("4. Delete idea");
        Console.WriteLine("5. Reorder all IDs");  // New option
        Console.WriteLine("6. Logout");
        Console.WriteLine("7. Exit");
        Console.Write("Select an option: ");

        var choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    await CreateIdea(repository);
                    break;

                case "2":
                    await ViewAllIdeas(repository);
                    break;

                case "3":
                    await UpdateIdea(repository);
                    break;

                case "4":
                    await DeleteIdea(repository);
                    break;

                case "5":
                    await ReorderAllIds(repository);
                    break;

                case "6":
                    Console.Write("Would you like to remove saved credentials? (y/n): ");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        credentialManager.RemoveCredentials();
                        Console.WriteLine("Credentials removed successfully.");
                    }
                    return;

                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to connect to database: {ex.Message}");
    return;
}

DatabaseCredentials PromptForCredentials()
{
    Console.WriteLine("Database Login");
    Console.Write("Server: ");
    var server = Console.ReadLine() ?? "";

    Console.Write("Port: ");
    var port = int.Parse(Console.ReadLine() ?? "3306");

    Console.Write("Database: ");
    var database = Console.ReadLine() ?? "";

    Console.Write("Username: ");
    var username = Console.ReadLine() ?? "";

    Console.Write("Password: ");
    var password = Console.ReadLine() ?? "";

    return new DatabaseCredentials
    {
        Server = server,
        Port = port,
        Database = database,
        Username = username,
        Password = password
    };
}

async Task CreateIdea(IIdeaRepository repository)
{
    Console.Write("Enter your idea: ");
    var content = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(content))
    {
        Console.WriteLine("Idea content cannot be empty");
        return;
    }
    var newIdea = await repository.CreateIdeaAsync(content);
    Console.WriteLine($"Idea created with ID: {newIdea.Id}");
}

async Task ViewAllIdeas(IIdeaRepository repository)
{
    var ideas = await repository.GetAllIdeasAsync();
    if (!ideas.Any())
    {
        Console.WriteLine("No ideas found");
        return;
    }
    foreach (var idea in ideas)
    {
        Console.WriteLine($"ID: {idea.Id}");
        Console.WriteLine($"Content: {idea.Content}");
        Console.WriteLine($"Created: {idea.CreatedDate}");
        Console.WriteLine($"Modified: {idea.ModifiedDate}");
        Console.WriteLine("------------------------");
    }
}

async Task UpdateIdea(IIdeaRepository repository)
{
    Console.Write("Enter idea ID to update: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Invalid ID format");
        return;
    }

    var idea = await repository.GetIdeaByIdAsync(id);
    if (idea == null)
    {
        Console.WriteLine($"Idea with ID {id} does not exist.");
        Console.Write("Would you like to create a new idea with this ID? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            Console.Write("Enter idea content: ");
            var content = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Content cannot be empty");
                return;
            }
            await repository.CreateIdeaAsync(id, content);
            Console.WriteLine($"New idea created with ID: {id}");
        }
        return;
    }

    while (true)
    {
        Console.WriteLine($"\nCurrent idea details:");
        Console.WriteLine($"ID: {idea.Id}");
        Console.WriteLine($"Content: {idea.Content}");
        Console.WriteLine($"Created: {idea.CreatedDate}");
        Console.WriteLine($"Modified: {idea.ModifiedDate}");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("(C)ancel - Cancel update");
        Console.WriteLine("(E)dit - Edit content");
        Console.WriteLine("(R)eidentify - Change idea ID");
        Console.Write("\nSelect an option: ");

        var choice = Console.ReadLine()?.ToLower();
        switch (choice)
        {
            case "c":
                return;

            case "e":
                Console.Write("Enter new content: ");
                var newContent = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newContent))
                {
                    Console.WriteLine("Content cannot be empty");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                await repository.UpdateIdeaAsync(id, newContent);
                Console.WriteLine("Content updated successfully");
                return;

            case "r":
                Console.Write("Enter new ID: ");
                if (!int.TryParse(Console.ReadLine(), out int newId))
                {
                    Console.WriteLine("Invalid ID format");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                try
                {
                    await repository.ChangeIdeaIdAsync(id, newId);
                    Console.WriteLine("ID updated successfully");
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }

            default:
                Console.WriteLine("Invalid option");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                break;
        }
    }
}
async Task DeleteIdea(IIdeaRepository repository)
{
    Console.Write("Enter idea ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Invalid ID format");
        return;
    }

    try
    {
        await repository.DeleteIdeaAsync(id);
        Console.WriteLine("Idea deleted successfully");
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

async Task ReorderAllIds(IIdeaRepository repository)
{
    Console.WriteLine("\nReorder IDs Operation");
    Console.WriteLine("This will reorganize all idea IDs sequentially based on creation date.");
    Console.WriteLine("For example:");
    Console.WriteLine("Current IDs:  1, 2, 4, 7");
    Console.WriteLine("New IDs:      1, 2, 3, 4");
    Console.WriteLine("\nAre you sure you want to reorder all IDs? (y/n): ");

    var response = Console.ReadLine()?.ToLower();
    if (response != "y")
    {
        Console.WriteLine("Reorder operation cancelled.");
        return;
    }

    await repository.ReorderIdsAsync();
    Console.WriteLine("All IDs have been successfully reordered.");
    Console.WriteLine("Press Enter to continue...");
    Console.ReadLine();
}