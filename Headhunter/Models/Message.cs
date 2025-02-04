﻿namespace Headhunter.Models;

public class Message
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ChatId { get; set; }
    public Chat? Chat { get; set; }
}
