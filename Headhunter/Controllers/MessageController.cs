using Headhunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headhunter.Controllers;

public class MessageController : Controller
{
    private readonly Context _context;
    private readonly UserManager<User> _userManager;
    public MessageController(Context context, UserManager<User> manager)
    {
        _context = context;
        _userManager = manager;
    }
    [HttpPost]
    public async Task<IActionResult> Create(int chatId, string body)
    {
        Chat chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id ==  chatId);
        User user = await _userManager.GetUserAsync(User);
        if (chat != null && user != null)
        {
            Message msg = new()
            {
                ChatId = chatId,
                Body = body,
                UserId = user.Id
            };
            await _context.AddAsync(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Chat", new {chatId = chatId});
        }
        return NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> GetMessages(int chatId)
    {
        var messages = await _context.Messages
            .Include(m => m.User)
            .Include(m => m.Chat)
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.Created)
            .Take(30)
            .ToListAsync();

        return PartialView("_MessagesPartial", messages);
    }
}
