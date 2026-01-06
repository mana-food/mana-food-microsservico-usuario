using System.Collections.Concurrent;
using ManaFood.Application.Interfaces.Services;

namespace ManaFood.Infrastructure.Auth
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private static readonly ConcurrentDictionary<string, bool> Blacklist = new();

        public void Add(string token)
        {
            Blacklist[token] = true;
        }

        public bool IsBlacklisted(string token)
        {
            return Blacklist.ContainsKey(token);
        }
    }
}
