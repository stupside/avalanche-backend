using Avalanche.Licence.Application.Domain.ValueTypes;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Licence.Application.Features;

public static class GetTicket
{
    [Serializable]
    public record Request(Guid OrganizationId, Guid UserId) : IRequest<Response>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.OrganizationId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AvalancheLicenceContext _context;

        public Handler(AvalancheLicenceContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Validities
                .Where(m => m.OrganizationId == request.OrganizationId && m.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            return new Response
            {
                OrganizationId = request.OrganizationId,
                Validities = ticket.Select(m => new TimeSpanStatus(m.From, m.To))
            };
        }
    }


    [Serializable]
    public class Response
    {
        public required Guid OrganizationId { get; init; }

        public bool IsValid => Validities.Any(m => m.Kind == TimeSpanStatus.StatusKind.Valid);

        public required IEnumerable<TimeSpanStatus> Validities { get; init; }
    }
}