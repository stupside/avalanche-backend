using Avalanche.Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Identity.Api.Controllers;

[Route("/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _user;

    public AccountController(IUserService user)
    {
        _user = user;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> Register(string username, string password)
    {
        var id = await _user.RegisterAsync(username, password);

        return id;
    }
}