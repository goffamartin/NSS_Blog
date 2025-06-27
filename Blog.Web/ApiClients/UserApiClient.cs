using Blog.Shared.Dtos;

namespace Blog.Web.ApiClients
{
    public class UserApiClient(HttpClient http)
    {
        public async Task<List<UserDto>> GetAllAsync()
        {
            return await http.GetFromJsonAsync<List<UserDto>>("api/users")
                   ?? new List<UserDto>();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return await http.GetFromJsonAsync<UserDto>($"api/users/{id}");
        }

        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            var response = await http.PostAsJsonAsync("api/users", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserDto>()
                   ?? throw new InvalidOperationException("Failed to parse created user.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await http.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }
    }

}
