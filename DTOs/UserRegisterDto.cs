using SafeScribe.Api.Models;

namespace SafeScribe.Api.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Role Role { get; set; } = Role.Leitor;
    }
}
