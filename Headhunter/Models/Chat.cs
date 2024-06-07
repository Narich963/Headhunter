namespace Headhunter.Models;

public class Chat
{
    public int Id { get; set; }

    public int EmployerId { get; set; }
    public User? Employer { get; set; }

    public int EmployeeId { get; set; }
    public User? Employee { get; set; }

    public List<Message> Messages { get; set; }
    public Chat()
    {
        Messages = new List<Message>();
    }
}
