namespace IdeaManagement.Models;

public class DatabaseCredentials
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string GetConnectionString()
    {
        return $"server={Server};port={Port};database={Database};user={Username};password={Password}";
    }
}
