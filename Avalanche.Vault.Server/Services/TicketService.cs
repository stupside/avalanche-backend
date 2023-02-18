using Avalanche.Identity.Client;
using Avalanche.Vault.Ticket;
using Avalanche.Vault.Application.Features;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Avalanche.Vault.Server.Services;

[Authorize]
public class TicketService : Vault.Ticket.TicketService.TicketServiceBase
{
    private readonly ISender _sender;

    public TicketService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GetOneTicketRpc.Types.Response> GetOne(GetOneTicketRpc.Types.Request request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        if (Guid.TryParse(request.TicketId, out var ticket) is false)
            throw new ValidationException("Invalid guid");

        var command = new GetTicket.Request(ticket, user);

        var response = await _sender.Send(command, context.CancellationToken);

        return new GetOneTicketRpc.Types.Response
        {
            StoreId = response.StoreId.ToString(),
            TicketId = response.TicketId.ToString(),
            Validities =
            {
                response.Validities.Select(m => new GetOneTicketRpc.Types.Response.Types.Validity
                {
                    From = m.From.ToTimestamp(),
                    To = m.To.ToTimestamp(),
                    Span = m.Span.ToDuration(),
                    Kind = (GetOneTicketRpc.Types.Response.Types.ValidityKind)m.Kind
                })
            },
            IsValid = response.IsValid
        };
    }

    public override async Task<GetManyTicketsRpc.Types.Response> GetMany(GetManyTicketsRpc.Types.Request request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        var command = new GetTickets.Request(user);

        var response = await _sender.Send(command, context.CancellationToken);

        var items = response.Select(m => new GetManyTicketsRpc.Types.Response.Types.Item
        {
            StoreId = m.StoreId.ToString(),
            TicketId = m.TicketId.ToString(),
            IsValid = m.IsValid
        });

        return new GetManyTicketsRpc.Types.Response
        {
            Items = { items }
        };
    }
}