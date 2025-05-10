using IdeaManagement.Data;
using IdeaManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Configure services
services.AddDbContext<IdeaDbContext>(options =>
    options.UseMySql(
        "server=dev0.notohost.eu;port=5506;database=itc_hmht6kb6;user=itc_user_hmht6kb6;password=fxf2t8wJywYj",
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

services.AddScoped<IIdeaRepository, IdeaRepository>();

var serviceProvider = services.BuildServiceProvider();

// Ensure database is created
using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdeaDbContext>();
    await context.Database.EnsureCreatedAsync();
}

var repository = serviceProvider.GetRequiredService<IIdeaRepository>();

while (true)
{
    Console.WriteLine("\nIdea Management System");
    Console.WriteLine("1. Create new idea");
    Console.WriteLine("2. View all ideas");
    Console.WriteLine("3. Update idea");
    Console.WriteLine("4. Delete idea");
    Console.WriteLine("5. Exit");
    Console.Write("Select an option: ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("Enter your idea: ");
                var content = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("Idea content cannot be empty");
                    break;
                }
                var newIdea = await repository.CreateIdeaAsync(content);
                Console.WriteLine($"Idea created with ID: {newIdea.Id}");
                break;

            case "2":
                var ideas = await repository.GetAllIdeasAsync();
                if (!ideas.Any())
                {
                    Console.WriteLine("No ideas found");
                    break;
                }
                foreach (var idea in ideas)
                {
                    Console.WriteLine($"ID: {idea.Id}");
                    Console.WriteLine($"Content: {idea.Content}");
                    Console.WriteLine($"Created: {idea.CreatedDate}");
                    Console.WriteLine($"Modified: {idea.ModifiedDate}");
                    Console.WriteLine("------------------------");
                }
                break;

            case "3":
                Console.Write("Enter idea ID to update: ");
                if (int.TryParse(Console.ReadLine(), out int updateId))
                {
                    Console.Write("Enter new content: ");
                    var newContent = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newContent))
                    {
                        Console.WriteLine("New content cannot be empty");
                        break;
                    }
                    await repository.UpdateIdeaAsync(updateId, newContent);
                    Console.WriteLine("Idea updated successfully");
                }
                else
                {
                    Console.WriteLine("Invalid ID format");
                }
                break;

            case "4":
                Console.Write("Enter idea ID to delete: ");
                if (int.TryParse(Console.ReadLine(), out int deleteId))
                {
                    await repository.DeleteIdeaAsync(deleteId);
                    Console.WriteLine("Idea deleted successfully");
                }
                else
                {
                    Console.WriteLine("Invalid ID format");
                }
                break;

            case "5":
                return;

            default:
                Console.WriteLine("Invalid option");
                break;
        }
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}
