using Avalanche.Licence.Application.Features;
using Avalanche.Ticket.Validity;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Avalanche.Licence.Server.Rpc;

public class ValidityRpc : ValidityService.ValidityServiceBase
{
    private readonly ISender _sender;

    public ValidityRpc(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<Challenge.Types.Response> Challenge(Challenge.Types.Request request,
        ServerCallContext context)
    {
        var organization = Guid.Parse(request.OrganizationId);
        var user = Guid.Parse(request.UserId);

        var response = await _sender.Send(new GetTicket.Request(organization, user));

        return new Challenge.Types.Response
        {
            Validities =
            {
                response.Validities.Select(m => new Challenge.Types.Response.Types.Validity
                {
                    From = m.From.ToTimestamp(),
                    To = m.To.ToTimestamp(),
                    Span = m.Span.ToDuration(),
                    Kind = (Challenge.Types.Response.Types.ValidityKind)m.Kind
                })
            },
            IsValid = response.IsValid
        };
    }
}