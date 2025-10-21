namespace SafeScribe.Api.Services
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklistAsync(string jti);
        Task<bool> IsBlacklistedAsync(string jti);
    }
}
