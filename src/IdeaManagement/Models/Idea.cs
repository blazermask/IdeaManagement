namespace IdeaManagement.Models;

public class Idea
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
