using Blog.AuthService.Model;
using Microsoft.AspNetCore.Identity;

namespace Blog.ApiService.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task InitializeAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = ["Admin", "User"];
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            async Task CreateUser(string id, string username, string email, string password, string? role = null)
            {
                if (await userManager.FindByEmailAsync(email) is null)
                {
                    var user = new User
                    {
                        Id = id,
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, password);

                    if (role is not null)
                        await userManager.AddToRoleAsync(user, role);
                }
            }

            await CreateUser("u-admin-001", "Alice Admin", "admin@blog.com", "Admin123!", "Admin");
            await CreateUser("u-author-002", "Bob Author", "user1@blog.com", "User123!");
            await CreateUser("u-user-003", "Charlie User", "user2@blog.com", "User123!");
        }
    }

}
