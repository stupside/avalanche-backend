using Avalanche.Identity.Application.Domain;

namespace Avalanche.Identity.Application.Services;

public interface IUserService
{
    public Task<User?> FindByUsername(string username);
    public Task<Guid> RegisterAsync(string username, string password);
    public Task<bool> VerifyAsync(User user, string password);
}