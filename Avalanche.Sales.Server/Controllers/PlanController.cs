using Avalanche.Identity.Client;
using Avalanche.Sales.Application.Features.Plans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Sales.Server.Controllers;

[Route("/plans")]
[ApiController]
public class PlanController : ControllerBase
{
    private readonly ISender _sender;

    public PlanController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{planId:guid}")]
    public async Task<ActionResult> Order(Guid planId, uint availableInDays, HttpContext context)
    {
        var command = new OrderPlan.Command(planId, context.GetUserId(),
            DateTimeOffset.UtcNow.AddDays(availableInDays).AddSeconds(30));

        await _sender.Send(command);

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(Guid catalogId, string name, TimeSpan validity, uint price,
        HttpContext context)
    {
        var command = new CreatePlan.Command(catalogId, name, validity, price);

        var response = await _sender.Send(command);

        return Ok(response);
    }

    [HttpDelete("{planId:guid}")]
    public async Task<ActionResult> Delete(Guid planId)
    {
        var command = new DeletePlan.Command(planId);

        await _sender.Send(command);

        return Ok();
    }
}