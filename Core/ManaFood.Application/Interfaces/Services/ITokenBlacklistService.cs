// Esta interface permite extensão para diferentes estratégias de blacklist.
namespace ManaFood.Application.Interfaces.Services;

public interface ITokenBlacklistService
{
    void Add(string token);
    bool IsBlacklisted(string token);
}