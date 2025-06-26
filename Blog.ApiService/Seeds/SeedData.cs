using Blog.ApiService.Data;
using Blog.ApiService.Models;

namespace Blog.ApiService.Seeds
{
    public static class SeedData
    {
        public static async Task InitializeAsync(BlogDbContext db)
        {
            if (db.Users.Any()) return;

            // Roles
            var adminRole = new Role { Name = "Admin" };
            var authorRole = new Role { Name = "Author" };
            var userRole = new Role { Name = "User" };

            db.Roles.AddRange(adminRole, authorRole, userRole);

            // Categories
            var categories = new[]
            {
            new Category { Name = "Technology" },
            new Category { Name = "Travel" },
            new Category { Name = "Education" }
        };
            db.Categories.AddRange(categories);

            // Tags
            var tags = new[]
            {
            new Tag { Name = "blazor" },
            new Tag { Name = "aspnet" },
            new Tag { Name = "cloud" }
        };
            db.Tags.AddRange(tags);

            // Users
            var admin = new User
            {
                Username = "Alice Admin",
                Email = "admin@example.com",
                IdentityProviderExternalId = "12345",
                Created = DateTime.UtcNow,
                UserRoles = [new() { Role = adminRole }]
            };

            var author = new User
            {
                Username = "Bob Author",
                Email = "author@example.com",
                IdentityProviderExternalId = "23456",
                Created = DateTime.UtcNow,
                UserRoles = [new() { Role = authorRole }]
            };

            var reader = new User
            {
                Username = "Charlie User",
                Email = "user@example.com",
                IdentityProviderExternalId = "34567",
                Created = DateTime.UtcNow,
                UserRoles = [new() { Role = userRole }]
            };

            db.Users.AddRange(admin, author, reader);
            await db.SaveChangesAsync();

            // Articles
            var article1 = new Article
            {
                Title = "Intro to Blazor",
                Content = "Learn Blazor step by step.",
                Author = author,
                Created = DateTime.UtcNow,
                Category = categories[0],
                Tags = [tags[0], tags[1]]
            };

            var article2 = new Article
            {
                Title = "Travel to Japan",
                Content = "What to expect on your first trip.",
                Author = author,
                Created = DateTime.UtcNow.AddDays(-5),
                Category = categories[1],
                Tags = [tags[2]]
            };

            db.Articles.AddRange(article1, article2);

            // Comments
            db.Comments.Add(new Comment
            {
                Article = article1,
                Author = reader,
                Content = "This helped a lot!",
                Created = DateTime.UtcNow
            });

            // Likes
            db.Likes.Add(new Like
            {
                Article = article1,
                User = reader,
                Created = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }

}
