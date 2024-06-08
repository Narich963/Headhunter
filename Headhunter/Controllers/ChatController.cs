using Headhunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headhunter.Controllers;

public class ChatController : Controller
{
    private readonly Context _context;
    private readonly UserManager<User> _userManager;
    public ChatController(Context context, UserManager<User> manager)
    {
        _context = context;
        _userManager = manager;
    }
    public async Task<IActionResult> Index()
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var chats = new List<Chat>();
            if (user.Role == "Соискатель")
            {
                chats = await _context.Chats
                    .Include(c => c.Employee)
                    .Include(c => c.Employer)
                    .Where(c => c.EmployeeId == user.Id).ToListAsync();
            }
            else
            {
                chats = await _context.Chats
                    .Include(c => c.Employee)
                    .Include(c => c.Employer)
                    .Where(c => c.EmployerId == user.Id).ToListAsync();
            }
            return View(chats);
        }
        return NotFound();
    }
    public async Task<IActionResult> Details(int chatId)
    {
        Chat chat = await _context.Chats
            .Include(c => c.Employee)
            .Include(c => c.Employer)
            .Include(c => c.Vacancy)
            .Include(c => c.Resume)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id ==  chatId);

        if (chat != null)
        {
            return PartialView("_ChatPartial", chat);
        }
        return NotFound();
    }
}
