using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Profiles;
using Blog.ApiService.Services;
using Blog.Shared.Dtos;

namespace Blog.Tests.Services
{
    [TestClass]
    public class CommentServiceTests
    {
        private CommentService _service = null!;
        private BlogDbContext _db = null!;
        private IMapper _mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = InMemoryDbContextFactory.Create();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommentProfile());
            });
            _mapper = config.CreateMapper();

            _service = new CommentService(_db, _mapper);
        }

        [TestMethod]
        public async Task CreateComment_Succeeds()
        {
            var dto = new CommentDto
            {
                Content = "Test comment",
                AuthorId = 1,
                ArticleId = 1
            };

            var created = await _service.CreateAsync(dto);

            Assert.AreEqual("Test comment", created.Content);
        }

        [TestMethod]
        public async Task GetByArticle_ReturnsExpected()
        {
            await _service.CreateAsync(new CommentDto { Content = "A", AuthorId = 1, ArticleId = 10 });
            await _service.CreateAsync(new CommentDto { Content = "B", AuthorId = 2, ArticleId = 20 });

            var result = await _service.GetByArticleIdAsync(10);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("A", result.First().Content);
        }

        [TestMethod]
        public async Task SoftDelete_RemovesFromResults()
        {
            var created = await _service.CreateAsync(new CommentDto { Content = "X", AuthorId = 1, ArticleId = 100 });
            var result = await _service.SoftDeleteAsync(created.Id);

            var remaining = await _service.GetByArticleIdAsync(100);

            Assert.IsTrue(result);
            Assert.AreEqual(0, remaining.Count());
        }
    }
}
