using CourseManagement.Api.Auth;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Api.Data;

public static class IdentitySeed
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var roleName in AuthConstants.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)
                {
                    Id = Guid.NewGuid(),
                    NormalizedName = roleName.ToUpperInvariant()
                });
            }
        }
    }
}
