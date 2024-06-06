using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Headhunter.Models;

namespace Headhunter.Controllers
{
    public class ModuleController : Controller
    {
        private readonly Context _context;

        public ModuleController(Context context)
        {
            _context = context;
        }

        // GET: Module
        public async Task<IActionResult> Index()
        {
            var module = _context.Modules.Include(c => c.Resume);
            return View(await module.ToListAsync());
        }

        // GET: Module/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Resume)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Module/Create
        public IActionResult Create(int? resumeId)
        {
            if (resumeId != null)
            {
                ViewBag.ResumeId = resumeId;
            }
            return PartialView("_CreatePartial");
        }

        // POST: Module/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Module module, int? resumeId)
        { 
            if (ModelState.IsValid)
            {
                _context.Add(module);
                await _context.SaveChangesAsync();
                ResumeController.Modules.Add(module);
                return Redirect("javascript:history.go(-2)");
            }
            if (resumeId != null)
            {
                ViewBag.ResumeId = resumeId;
            }
            return PartialView("_CreatePartial", module);
        }

        // GET: Module/Edit/5
        public async Task<IActionResult> Edit(int moduleId)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: Module/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Module module)
        {
            var oldModule = await _context.Modules.Include(o => o.Resume).AsNoTracking().FirstOrDefaultAsync(m => m.Id == module.Id);
            module.Resume = oldModule.Resume;
            module.ResumeId = oldModule.ResumeId;
            if (ModelState.IsValid)
            {
                try
                {
                    module.Resume.Updated = DateTime.UtcNow;
                    _context.Update(module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Resume", new {id = module.ResumeId});
            }
            return View(module);
        }

        // GET: Module/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Resume)
                .FirstOrDefaultAsync(m => m.Id == id);

            module.Resume.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module != null)
            {
                _context.Modules.Remove(module);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", controllerName: "Resume", routeValues: new {id = module.ResumeId});
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
