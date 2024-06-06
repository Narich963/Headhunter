namespace Headhunter.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Salary { get; set; }
    public string Description { get; set; }
    public int ExpFrom { get; set; }
    public int ExpTo { get; set; }
    public string Category { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; }
    public bool IsPublished { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}
