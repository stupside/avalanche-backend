using Avalanche.Corporation.Application.Features.Invitations;
using Avalanche.Identity.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Corporation.Server.Controllers;

[Route("/invitations")]
[ApiController]
public sealed class InvitationController : ControllerBase
{
    private readonly ISender _sender;

    public InvitationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Send(Guid organizationId, string email)
    {
        var command = new SendInvitation.Command(organizationId, email);

        var response = await _sender.Send(command);

        return Ok(response);
    }

    [HttpPost("{invitationId:guid}")]
    public async Task<ActionResult> Accept(Guid invitationId, HttpContext context)
    {
        var command = new AcceptInvitation.Command(invitationId, context.GetUserId());

        await _sender.Send(command);

        return Ok();
    }

    [HttpPost("{invitationId:guid}")]
    public async Task<ActionResult> Reject(Guid invitationId)
    {
        var command = new RejectInvitation.Command(invitationId);

        await _sender.Send(command);

        return Ok();
    }
}