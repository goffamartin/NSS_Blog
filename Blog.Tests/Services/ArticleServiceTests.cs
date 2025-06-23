using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Dtos;
using Blog.ApiService.Profiles;
using Blog.ApiService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Tests.Services
{
    [TestClass]
    public class ArticleServiceTests
    {
        private IArticleService _service = null!;
        private BlogDbContext _db = null!;
        private IMapper _mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = InMemoryDbContextFactory.Create();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ArticleProfile());
            });

            _mapper = config.CreateMapper();
            _service = new ArticleService(_db, _mapper);
        }

        [TestMethod]
        public async Task CreateArticle_Succeeds()
        {
            var dto = new ArticleDto
            {
                Title = "Test Title",
                Content = "Test Content",
                AuthorId = 1,
                CategoryId = null
            };

            var result = await _service.CreateAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Title", result.Title);
        }

        [TestMethod]
        public async Task GetAll_ReturnsList()
        {
            await _service.CreateAsync(new ArticleDto { Title = "X", Content = "Y", AuthorId = 1 });

            var list = await _service.GetAllAsync();

            Assert.AreEqual(1, list.Count());
        }

        [TestMethod]
        public async Task GetByAuthor_ReturnsCorrectArticles()
        {
            await _service.CreateAsync(new ArticleDto { Title = "A1", Content = "x", AuthorId = 1 });
            await _service.CreateAsync(new ArticleDto { Title = "A2", Content = "y", AuthorId = 2 });

            var result = await _service.GetByAuthorAsync(1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("A1", result.First().Title);
        }

        [TestMethod]
        public async Task Search_ReturnsMatchingArticles()
        {
            await _service.CreateAsync(new ArticleDto { Title = "Blazor Intro", Content = "...", AuthorId = 1 });
            await _service.CreateAsync(new ArticleDto { Title = "EF Core Guide", Content = "...", AuthorId = 1 });

            var result = await _service.SearchAsync("Blazor");

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Blazor Intro", result.First().Title);
        }

        [TestMethod]
        public async Task SoftDeleteByAuthor_RemovesAll()
        {
            await _service.CreateAsync(new ArticleDto { Title = "X", Content = "Y", AuthorId = 3 });
            await _service.CreateAsync(new ArticleDto { Title = "Z", Content = "Y", AuthorId = 3 });

            var count = await _service.SoftDeleteByAuthorAsync(3);
            var remaining = await _service.GetByAuthorAsync(3);

            Assert.AreEqual(2, count);
            Assert.AreEqual(0, remaining.Count());
        }
    }
}
