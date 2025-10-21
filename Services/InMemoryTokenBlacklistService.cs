using System.Collections.Concurrent;

namespace SafeScribe.Api.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, byte> _set = new();

        public Task AddToBlacklistAsync(string jti)
        {
            _set.TryAdd(jti, 0);
            return Task.CompletedTask;
        }

        public Task<bool> IsBlacklistedAsync(string jti)
            => Task.FromResult(_set.ContainsKey(jti));
    }
}
