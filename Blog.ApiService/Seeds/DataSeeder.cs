using Blog.ApiService.Data;
using Blog.ApiService.Models;

namespace Blog.ApiService.Seeds
{
    public static class DataSeeder
    {
        public static async Task InitializeAsync(BlogDbContext db)
        {
            //if (db.Users.Any()) return;

            var now = DateTime.UtcNow;

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
            new Tag { Name = "cloud" },
            new Tag { Name = "adventure" }
        };
            db.Tags.AddRange(tags);

            // Users (linked to Identity)
            var admin = new User
            {
                Username = "Alice Admin",
                Email = "admin@example.com",
                IdentityProviderExternalId = "u-admin-001",
                Created = now
            };

            var author = new User
            {
                Username = "Bob Author",
                Email = "author@example.com",
                IdentityProviderExternalId = "u-author-002",
                Created = now
            };

            var user = new User
            {
                Username = "Charlie User",
                Email = "user@example.com",
                IdentityProviderExternalId = "u-user-003",
                Created = now
            };

            db.Users.AddRange(admin, author, user);
            await db.SaveChangesAsync();

            // Articles
            var article1 = new Article
            {
                Title = "Intro to Blazor",
                Content = "Learn Blazor step by step.",
                Author = author,
                Created = now.AddDays(-10),
                Category = categories[0],
                Tags = [tags[0], tags[1]]
            };

            var article2 = new Article
            {
                Title = "Climbing Mount Fuji",
                Content = "Adventures from Japan.",
                Author = author,
                Created = now.AddDays(-5),
                Category = categories[1],
                Tags = [tags[3]]
            };

            var article3 = new Article
            {
                Title = "ASP.NET Core Minimal APIs",
                Content = "Modern approach to building APIs.",
                Author = admin,
                Created = now.AddDays(-3),
                Category = categories[0],
                Tags = [tags[1], tags[2]]
            };

            var article4 = new Article
            {
                Title = "Study Tips for Developers",
                Content = "How to stay productive while learning.",
                Author = user,
                Created = now.AddDays(-1),
                Category = categories[2],
                Tags = [tags[0], tags[2]]
            };

            db.Articles.AddRange(article1, article2, article3, article4);
            await db.SaveChangesAsync();

            // Comments
            db.Comments.AddRange(
                new Comment
                {
                    Article = article1,
                    Author = user,
                    Content = "Super useful, thanks!",
                    Created = now.AddDays(-9)
                },
                new Comment
                {
                    Article = article1,
                    Author = admin,
                    Content = "Great intro, good job.",
                    Created = now.AddDays(-8)
                },
                new Comment
                {
                    Article = article2,
                    Author = user,
                    Content = "Inspiring post!",
                    Created = now.AddDays(-4)
                },
                new Comment
                {
                    Article = article4,
                    Author = author,
                    Content = "Nice tips, I’ll try them.",
                    Created = now.AddDays(-1)
                }
            );

            // Likes
            db.Likes.AddRange(
                new Like { Article = article1, User = user, Created = now.AddDays(-8) },
                new Like { Article = article1, User = admin, Created = now.AddDays(-7) },
                new Like { Article = article2, User = user, Created = now.AddDays(-4) },
                new Like { Article = article3, User = user, Created = now.AddDays(-2) },
                new Like { Article = article3, User = admin, Created = now.AddDays(-2) }
            );

            await db.SaveChangesAsync();
        }
    }
}
