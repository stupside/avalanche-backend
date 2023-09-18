using Avalanche.Identity.Application.Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sodium;

namespace Avalanche.Identity.Application.Services;

public class UserService : IUserService
{
    private readonly IdentityContext _context;

    public UserService(IdentityContext context)
    {
        _context = context;
    }

    public Task<User?> FindByUsername(string username)
    {
        return _context.Users.SingleOrDefaultAsync(m => m.Username == username.Trim());
    }

    public async Task<Guid> RegisterAsync(string username, string password)
    {
        if (await _context.Users.AnyAsync(m => m.Username == username.Trim()))
            throw new ValidationException("Username already taken");

        var user = new User
        {
            Username = username.Trim(),
            Hash = PasswordHash.ArgonHashString(password.Trim(), PasswordHash.StrengthArgon.Medium)
        };

        var entry = await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return entry.Entity.Id;
    }

    public Task<bool> VerifyAsync(User user, string password)
    {
        return Task.FromResult(PasswordHash.ArgonHashStringVerify(user.Hash, password.Trim()));
    }
}