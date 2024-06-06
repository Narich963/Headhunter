namespace Headhunter.Models;

public class Module
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string PlaceName { get; set; }
    public string JobPosition { get; set; }
    public string Responsibilities { get; set; }
    public int WorkingYear { get; set; }

    public int? ResumeId { get; set; }
    public Resume? Resume { get; set; }
}
