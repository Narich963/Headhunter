using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Headhunter.Models;
using Microsoft.AspNetCore.Identity;

namespace Headhunter.Controllers
{
    public class ResumeController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
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
        public IActionResult Create(int? userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        // POST: Resume/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resume resume, int? userId)
        {
            if (userId == null) 
            {
                return NotFound();
            }
            User user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (user.Id == userId)
                {
                    resume.UserId = user.Id;
                    if (ModelState.IsValid)
                    {
                        _context.Add(resume);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", routeValues: userId, controllerName: "Employee");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            ViewBag.UserId = userId;
            return View(resume);
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", resume.UserId);
            return View(resume);
        }

        // POST: Resume/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,JobPosition,ExpectedSalary,Telegram,Facebook,LinkedIn,Created,Updated,IsPublished,UserId")] Resume resume)
        {
            if (id != resume.Id)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", resume.UserId);
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
            var resume = await _context.Resumes.FindAsync(id);
            if (resume != null)
            {
                _context.Resumes.Remove(resume);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResumeExists(int id)
        {
            return _context.Resumes.Any(e => e.Id == id);
        }
    }
}
