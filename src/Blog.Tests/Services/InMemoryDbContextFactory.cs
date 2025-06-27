using Blog.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Tests.Services
{
    public static class InMemoryDbContextFactory
    {
        public static BlogDbContext Create()
        {
            var options = new DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new BlogDbContext(options);
        }
    }
}
