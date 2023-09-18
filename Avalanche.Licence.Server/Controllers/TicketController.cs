using Avalanche.Identity.Client;
using Avalanche.Licence.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Licence.Server.Controllers;

[Authorize]
[Controller]
public class TicketController: ControllerBase
{
    private readonly ISender _sender;

    public TicketController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<ActionResult<GetTicket.Response>> GetOne(Guid organizationId, HttpContext context)
    {
        var request = new GetTicket.Request(organizationId, context.GetUserId());
        
        var response = await _sender.Send(request);

        return Ok(response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetTickets.Response>>> GetMany(HttpContext context)
    {
        var request = new GetTickets.Request(context.GetUserId());
        
        var response = await _sender.Send(request);

        return Ok(response);
    }
}