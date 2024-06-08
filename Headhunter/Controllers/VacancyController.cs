using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Headhunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Headhunter.Controllers;
[Authorize]
public class VacancyController : Controller
{
    private readonly Context _context;
    private readonly UserManager<User> _userManager;

    public VacancyController(Context context, UserManager<User> manager)
    {
        _context = context;
        _userManager = manager;
    }

    // GET: Vacancн
    public async Task<IActionResult> Index(int? userId)
    {
        if (userId != null)
        {
            User vacancyOwner = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (vacancyOwner != null)
            {
                var vacancies = new List<Vacancy>();
                if (vacancyOwner.UserName == User.Identity.Name)
                {
                    vacancies = await _context.Vacancies.Include(r => r.User)
                        .Where(r => r.UserId == userId)
                        .ToListAsync();
                }
                else
                {
                    vacancies = await _context.Vacancies.Include(r => r.User)
                        .Where(r => r.UserId == userId)
                        .Where(r => r.IsPublished == true)
                        .ToListAsync();
                }
                return PartialView("_VacanciesPartial", vacancies);
            }
        }
        return NotFound();
    }

    // GET: Vacancн/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vacancy = await _context.Vacancies
            .Include(v => v.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vacancy == null)
        {
            return NotFound();
        }

        return View(vacancy);
    }

    // GET: Vacancн/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Vacancн/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vacancy vacancy)
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            vacancy.UserId = user.Id;
            vacancy.Updated = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(vacancy);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new {id = vacancy.Id});
            }
        }
        return View(vacancy);
    }

    // GET: Vacancн/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vacancy = await _context.Vacancies.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == id);
        if (vacancy.User.UserName != User.Identity.Name)
        {
            return BadRequest();
        }
        if (vacancy == null)
        {
            return NotFound();
        }
        return View(vacancy);
    }

    // POST: Vacancн/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vacancy vacancy)
    {
        if (id != vacancy.Id)
        {
            return NotFound();
        }

        var oldVacancy = await _context.Vacancies.Include(o => o.User).AsNoTracking().FirstOrDefaultAsync(o => o.Id == vacancy.Id);
        if (oldVacancy.User.UserName != User.Identity.Name)
        {
            return BadRequest();
        }
        vacancy.UserId = oldVacancy.UserId;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vacancy);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacancyExists(vacancy.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new {id = vacancy.Id});
        }
        return View(vacancy);
    }

    // GET: Vacancн/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vacancy = await _context.Vacancies
            .Include(v => v.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vacancy.User.UserName != User.Identity.Name)
        {
            return BadRequest();
        }
        if (vacancy == null)
        {
            return NotFound();
        }

        return View(vacancy);
    }

    // POST: Vacancн/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        var userId = vacancy.UserId;
        if (vacancy != null)
        {
            if (vacancy.User.UserName != User.Identity.Name)
            {
                return BadRequest();
            }
            _context.Vacancies.Remove(vacancy);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "User", new {id = userId});
    }
    public async Task<IActionResult> Update(int id)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(r => r.Id == id);
        if (vacancy != null)
        {
            if (vacancy.User.UserName != User.Identity.Name)
            {
                return BadRequest();
            }
            vacancy.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Redirect("javascript:history.go(-1)");
        }
        return NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> Apply(int vacancyId)
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            if (user.Role == "Соискатель")
            {
                var resumes = await _context.Resumes
                    .Include(r => r.User)
                    .Where(r => r.UserId == user.Id)
                    .Where(r => r.IsPublished == true)
                    .ToListAsync();
                ViewBag.VacancyId = vacancyId;
                return PartialView("_ApplyPartial", resumes);
            }
            ModelState.AddModelError("", "Только соискатели могут откликаться на вакансии");
        }
        return NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Apply(int vacancyId, int resumeId)
    {
        Resume resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId);
        if (resume != null)
        {
            var vacancy = await _context.Vacancies.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == vacancyId);
            User employee = await _userManager.GetUserAsync(User);
            if (employee != null)
            {
                Chat chat = new()
                {
                    EmployeeId = employee.Id,
                    EmployerId = vacancy.UserId,
                    ResumeId = resume.Id,
                    VacancyId = vacancyId
                };
                await _context.AddAsync(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Chat");
            }
        }
        return NotFound();
    }
    private bool VacancyExists(int id)
    {
        return _context.Vacancies.Any(e => e.Id == id);
    }
}
