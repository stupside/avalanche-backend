using Avalanche.Sales.Application.Features.Catalogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Sales.Server.Controllers;

[Route("/catalogs")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(Guid organizationId, HttpContext context)
    {
        var command = new CreateCatalog.Command(organizationId);

        var response = await _sender.Send(command);

        return Ok(response);
    }
}