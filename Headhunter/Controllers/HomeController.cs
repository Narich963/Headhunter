using Headhunter.Models;
using Headhunter.Services;
using Headhunter.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Headhunter.Services.Order;

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

    public async Task<IActionResult> Index(string? category, string? name, int page = 1, Order order = SalaryAsc)
    {
        ViewBag.Sort = order == SalaryAsc ? SalaryDesc : SalaryAsc;
        User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        int pageSize = 2;

        if (user != null)
        {
            if (user.Role == "Соискатель")
            {
                var vacancies = await _context.Vacancies
                    .Include(v => v.User)
                    .Where(v => v.IsPublished)
                    .OrderBy(v => v.Updated)
                    .ToListAsync();

                switch (order)
                {
                    case SalaryAsc:
                        vacancies = vacancies.OrderBy(v => v.Salary).ToList();
                        break;
                    case SalaryDesc:
                        vacancies = vacancies.OrderByDescending(v => v.Salary).ToList();
                        break;
                }
                if (category != null)
                {
                    vacancies = vacancies.Where(c => c.Category == category).ToList();
                }
                if (name != null)
                {
                    vacancies = vacancies.Where(c => c.Name.Contains(name)).ToList();
                }

                var count = vacancies.Count();
                var items = vacancies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel model = new(count, page, pageSize);

                ResumeOrVacancy rov = new()
                {
                    Vacancies = items,
                    PageViewModel = model
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
                
                if (category != null)
                {
                    resumes = resumes.Where(c => c.Category == category).ToList();
                }
                if (name != null)
                {
                    resumes = resumes.Where(c => c.JobPosition.Contains(name)).ToList();
                }
                switch (order)
                {
                    case SalaryAsc:
                        resumes = resumes.OrderBy(v => v.ExpectedSalary).ToList();
                        break;
                    case SalaryDesc:
                        resumes = resumes.OrderByDescending(v => v.ExpectedSalary).ToList();
                        break;
                }

                var count = resumes.Count();
                var items = resumes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel model = new(count, page, pageSize);

                ResumeOrVacancy rov = new()
                {
                    Resumes = items,
                    PageViewModel = model
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
