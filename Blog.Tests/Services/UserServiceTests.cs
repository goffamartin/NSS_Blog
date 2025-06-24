using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Profiles;
using Blog.ApiService.Services;
using Blog.Shared.Dtos;

namespace Blog.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private BlogDbContext _db = null!;
        private IUserService _service = null!;
        private IMapper _mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = InMemoryDbContextFactory.Create();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });
            _mapper = config.CreateMapper();

            _service = new UserService(_db, _mapper);
        }

        [TestMethod]
        public async Task CreateUser_Succeeds()
        {
            var dto = new UserDto
            {
                DisplayName = "Tester",
                Email = "tester@example.com"
            };

            var created = await _service.CreateAsync(dto);

            Assert.AreEqual("Tester", created.DisplayName);
            Assert.AreEqual(false, created.Banned);
        }

        [TestMethod]
        public async Task GetAll_ReturnsUsers()
        {
            await _service.CreateAsync(new UserDto { DisplayName = "A", Email = "a@x.com" });
            await _service.CreateAsync(new UserDto { DisplayName = "B", Email = "b@x.com" });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsCorrectUser()
        {
            var created = await _service.CreateAsync(new UserDto { DisplayName = "X", Email = "x@x.com" });

            var result = await _service.GetByIdAsync(created.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("X", result?.DisplayName);
        }

        [TestMethod]
        public async Task SoftDelete_BansUser()
        {
            var user = await _service.CreateAsync(new UserDto { DisplayName = "BanMe", Email = "ban@me.com" });

            var deleted = await _service.SoftDeleteAsync(user.Id);
            var all = await _service.GetAllAsync();

            Assert.IsTrue(deleted);
            Assert.AreEqual(0, all.Count());
        }
    }
}
