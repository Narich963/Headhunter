using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Headhunter.Models;
using Microsoft.AspNetCore.Identity;

namespace Headhunter.Controllers;

public class ResumeController : Controller
{
    private readonly Context _context;
    private readonly UserManager<User> _userManager;
    public static List<Module> Modules { get; set; } = new List<Module>();
    public ResumeController(Context context, UserManager<User> manager)
    {
        _context = context;
        _userManager = manager;
    }

    // GET: Resume
    public async Task<IActionResult> Index(int? userId)
    {
        if (userId != null)
        {
            User resumeOwner = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (resumeOwner != null)
            {
                var resumes = new List<Resume>();
                if (resumeOwner.UserName == User.Identity.Name)
                {
                    resumes = await _context.Resumes.Include(r => r.User)
                        .Where(r => r.UserId == userId)
                        .ToListAsync();
                }
                else
                {
                    resumes = await _context.Resumes.Include(r => r.User)
                        .Where(r => r.UserId == userId)
                        .Where(r => r.IsPublished == true)
                        .ToListAsync();
                }
                return PartialView("_ResumesPartial", resumes);
            }
        }
        return NotFound();
    }

    // GET: Resume/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Resume/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Resume resume)
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            resume.UserId = user.Id;
            resume.Updated = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(resume);
                await _context.SaveChangesAsync();
                if (Modules.Count != 0)
                {
                    foreach (var m in Modules)
                    {
                        var module = await _context.Modules.Include(module => module.Resume).FirstOrDefaultAsync(module => module.Id == m.Id);
                        module.ResumeId = resume.Id;
                        _context.Update(module);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Details", routeValues: new {id = resume.Id});
            }
            _context.RemoveRange(Modules);
            await _context.SaveChangesAsync();
        }
        Modules.Clear();
        return View(resume);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id != null)
        {
            var resume = await _context.Resumes
                .Include(r => r.User)
                .Include(r => r.Modules)
                .FirstOrDefaultAsync(r => r.Id ==  id);

            if (resume != null)
            {
                return View(resume);
            }
        }
        return NotFound();
    }

    // GET: Resume/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await _context.Resumes.FindAsync(id);
        if (resume == null)
        {
            return NotFound();
        }
        return View(resume);
    }

    // POST: Resume/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Resume resume)
    {
        if (id != resume.Id)
        {
            return NotFound();
        }

        var oldResume = await _context.Resumes.AsNoTracking().FirstOrDefaultAsync(o => o.Id ==  resume.Id);
        resume.UserId = oldResume.UserId;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(resume);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResumeExists(resume.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new {id = resume.Id});
        }
        return View(resume);
    }

    // GET: Resume/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await _context.Resumes
            .Include(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (resume == null)
        {
            return NotFound();
        }

        return View(resume);
    }

    // POST: Resume/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resume = await _context.Resumes.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
        var userId = resume.UserId;
        if (resume != null)
        {
            _context.Resumes.Remove(resume);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "User", new {id = userId});
    }
    public async Task<IActionResult> Update(int? id)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);
        if (resume != null)
        {
            resume.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Redirect("javascript:history.go(-1)");
        }
        return NotFound();
    }
    private bool ResumeExists(int id)
    {
        return _context.Resumes.Any(e => e.Id == id);
    }
}
