using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace AI_WebsiteBuilder.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Access Pages table

            // Create Roles
            string[] roleNames = { "Admin", "Member" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create Admin User
            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];

            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
            {
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }

            // Create Member User
            var memberEmail = configuration["MemberUser:Email"];
            var memberPassword = configuration["MemberUser:Password"];

            if (!string.IsNullOrEmpty(memberEmail) && !string.IsNullOrEmpty(memberPassword))
            {
                if (await userManager.FindByEmailAsync(memberEmail) == null)
                {
                    var memberUser = new IdentityUser
                    {
                        UserName = memberEmail,
                        Email = memberEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(memberUser, memberPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(memberUser, "Member");
                    }
                }
            }

            // Seed Default Website Pages
            if (!dbContext.WebPages.Any())
            {
                dbContext.WebPages.AddRange(
                    new AI_WebsiteBuilder.Models.WebPage
                    {
                        Member = adminEmail ?? "admin@atu.ie",
                        PageName = "Home",
                        PageDescription = "Homepage description",
                        PageContent = "<h1>Welcome to the AI Website Builder!</h1>",
                        CreatedAt = DateTime.Now
                    },
                    new AI_WebsiteBuilder.Models.WebPage
                    {
                        Member = adminEmail ?? "admin@atu.ie",
                        PageName = "About",
                        PageDescription = "About us description",
                        PageContent = "<p>Information about our company.</p>",
                        CreatedAt = DateTime.Now
                    }
                );

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
