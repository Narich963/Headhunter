using Headhunter.Models;
using Headhunter.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Headhunter.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.Login) ?? await _userManager.FindByNameAsync(model.Login);
            if (user != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_userManager.Users.Any(u => u.Email == model.Email || u.UserName == model.Username))
            {
                ModelState.AddModelError("", "Пользователь с тамим именем или эл. почтой уже существует");
                return View(model);
            }
            User? user = new()
            {
                Email = model.Email,
                UserName = model.Username,
                Avatar = model.Avatar,
                Role = model.Role,
                PhoneNumber = model.ContactPhone,
                Name = model.Name
            };
            if (model.Avatar == null)
            {
                user.Avatar = "https://www.iconpacks.net/icons/2/free-user-icon-3296-thumb.png";
            }
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
