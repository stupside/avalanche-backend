using Avalanche.Exceptions;
using Avalanche.Vault.Application.Domain;
using Avalanche.Vault.Application.Domain.ValueTypes;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Vault.Application.Features;

public static class GetTicket
{
    [Serializable]
    public record Request(Guid TicketId, Guid UserId) : IRequest<Response>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.TicketId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AvalancheVaultContext _context;

        public Handler(AvalancheVaultContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets
                .SingleOrDefaultAsync(m => m.Id == request.TicketId && m.UserId == request.UserId, cancellationToken);

            if (ticket is null) throw new NotFoundException(typeof(Ticket), request.TicketId);

            var validities = await _context.Validities.Where(m => m.TicketId == ticket.Id)
                .ToListAsync(cancellationToken);

            return new Response
            {
                StoreId = ticket.StoreId,
                TicketId = ticket.Id,
                Validities = validities.Select(m => new TimeSpanStatus(m.From, m.To))
            };
        }
    }


    [Serializable]
    public class Response
    {
        public required Guid StoreId { get; init; }
        public required Guid TicketId { get; init; }

        public bool IsValid => Validities.Any(m => m.Kind == TimeSpanStatus.StatusKind.Valid);

        public required IEnumerable<TimeSpanStatus> Validities { get; init; }
    }
}