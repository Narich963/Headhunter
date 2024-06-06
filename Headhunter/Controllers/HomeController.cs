using Headhunter.Models;
using Headhunter.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Headhunter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly Context _context;

    public HomeController(ILogger<HomeController> logger, Context context, UserManager<User> manager)
    {
        _logger = logger;
        _context = context;
        _userManager = manager;
    }

    public async Task<IActionResult> Index()
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        if (user != null)
        {
            if (user.Role == "Соискатель")
            {
                var vacancies = await _context.Vacancies
                    .Include(v => v.User)
                    .Where(v => v.IsPublished == true)
                    .OrderBy(v => v.Updated)
                    .ToListAsync();
                ResumeOrVacancy rov = new()
                {
                    Vacancies = vacancies
                };
                return View(rov);
            }
            else
            {
                var resumes = await _context.Resumes
                    .Include(v => v.User)
                    .Where(v => v.IsPublished == true)
                    .OrderBy(v => v.Updated)
                    .ToListAsync();
                ResumeOrVacancy rov = new()
                {
                    Resumes = resumes
                };
                return View(rov);
            }
        }
        return NotFound();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
