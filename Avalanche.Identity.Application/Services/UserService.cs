using Avalanche.Identity.Application.Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sodium;

namespace Avalanche.Identity.Application.Services;

public class UserService : IUserService
{
    private readonly AvalancheIdentityContext _context;

    public UserService(AvalancheIdentityContext context)
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
            UserCredential = new UserCredential
            {
                Hash = PasswordHash.ArgonHashString(password.Trim(), PasswordHash.StrengthArgon.Medium)
            }
        };

        var entry = await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return entry.Entity.Id;
    }

    public async Task<bool> VerifyAsync(User user, string password)
    {
        var credential = user.UserCredential ??
                         await _context.UserCredentials.SingleOrDefaultAsync(m => m.Id == user.UserCredentialId);

        if (credential is null) return false;

        return PasswordHash.ArgonHashStringVerify(credential.Hash, password.Trim());
    }
}