using Avalanche.Corporation.Application.Features.Organizations;
using Avalanche.Identity.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Corporation.Server.Controllers;

[Route("/organizations")]
[ApiController]
public sealed class OrganizationController : ControllerBase
{
    private readonly ISender _sender;

    public OrganizationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(string name, HttpContext context)
    {
        var command = new CreateOrganization.Command(context.GetUserId(), name);

        var response = await _sender.Send(command);

        return response;
    }
}