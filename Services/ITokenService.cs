using SafeScribe.Api.Models;

namespace SafeScribe.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user, out string jti);
    }
}
