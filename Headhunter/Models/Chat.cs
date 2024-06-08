namespace Headhunter.Models;

public class Chat
{
    public int Id { get; set; }

    public int EmployerId { get; set; }
    public User? Employer { get; set; }

    public int EmployeeId { get; set; }
    public User? Employee { get; set; }
    
    public int? ResumeId { get; set; }
    public Resume? Resume { get; set; }

    public int? VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }

    public List<Message> Messages { get; set; }
    public Chat()
    {
        Messages = new List<Message>();
    }
}
