using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Profiles;
using Blog.ApiService.Services;
using Blog.Shared.Dtos;

namespace Blog.Tests.Services
{
    [TestClass]
    public class LikeServiceTests
    {
        private LikeService _service = null!;
        private BlogDbContext _db = null!;
        private IMapper _mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = InMemoryDbContextFactory.Create();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new LikeProfile());
            });
            _mapper = config.CreateMapper();

            _service = new LikeService(_db, _mapper);
        }

        [TestMethod]
        public async Task ToggleLike_AddsLike()
        {
            var dto = new LikeDto { ArticleId = 1, UserId = 1 };

            var result = await _service.ToggleLikeAsync(dto);
            var count = await _service.GetLikeCountAsync(1);

            Assert.IsTrue(result);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task ToggleLike_RemovesLike()
        {
            var dto = new LikeDto { ArticleId = 2, UserId = 2 };
            await _service.ToggleLikeAsync(dto); // Add first

            var result = await _service.ToggleLikeAsync(dto); // Should remove
            var count = await _service.GetLikeCountAsync(2);

            Assert.IsFalse(result);
            Assert.AreEqual(0, count);
        }
    }
}
