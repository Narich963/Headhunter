using Headhunter.Models;
using Microsoft.AspNetCore.Identity;

namespace Headhunter.Services;

public class RolesInitializer
{
    public static async Task SeedRoles(RoleManager<IdentityRole<int>> _roleManager)
    {
        var roles = new[] { "Работодатель", "Соискатель" };

        foreach (var role in roles)
        {
            if (await _roleManager.FindByNameAsync(role) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }
}
