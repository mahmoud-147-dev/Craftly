using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Seeder
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@test.com";
            var adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin != null)
                return;

            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin"
            };

            var result = await userManager.CreateAsync(
                admin,
                adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
