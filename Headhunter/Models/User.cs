using Microsoft.AspNetCore.Identity;

namespace Headhunter.Models;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string? Avatar { get; set; }
    public string Role { get; set; }

    public List<Resume> Resumes { get; set; }
    public User()
    {
        Resumes = new List<Resume>();   
    }
}