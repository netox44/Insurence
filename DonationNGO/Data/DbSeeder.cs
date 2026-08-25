using Microsoft.AspNetCore.Identity;

namespace Insurence.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdmin(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1️⃣ Create Roles if not exist
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // 2️⃣ Create default Admin account
            string adminEmail = "admin@site.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
