namespace Blog.Shared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? IdentityProviderExternalId { get; set; }
        public bool Banned { get; set; }
        public DateTime Created { get; set; }
    }
}
