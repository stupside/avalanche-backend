using Avalanche.Corporation.Application.Features.Members;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Corporation.Server.Controllers;

[Route("/members")]
[ApiController]
public sealed class MemberController : ControllerBase
{
    private readonly ISender _sender;

    public MemberController(ISender sender)
    {
        _sender = sender;
    }

    [HttpDelete("{memberId:guid}")]
    public async Task<ActionResult> Remove(Guid memberId)
    {
        var command = new RemoveMember.Command(memberId);

        await _sender.Send(command);

        return Ok();
    }
}