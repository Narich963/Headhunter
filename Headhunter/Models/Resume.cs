namespace Headhunter.Models;

public class Resume
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string JobPosition { get; set; }
    public int ExpectedSalary { get; set; }
    public string Telegram { get; set; }
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; }
    public bool IsPublished { get; set; } = false;

    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Module> Modules { get; set; }
    public Resume()
    {
        Modules = new List<Module>();
    }
}
