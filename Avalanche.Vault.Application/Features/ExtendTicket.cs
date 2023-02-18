using Avalanche.Vault.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Vault.Application.Features;

public static class ExtendTicket
{
    [Serializable]
    public record Command(Guid StoreId, Guid UserId, DateTimeOffset Availability, TimeSpan AvailabilitySpan) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.StoreId).NotEmpty();

            RuleFor(m => m.Availability).NotEmpty().GreaterThanOrEqualTo(DateTimeOffset.UtcNow);
            RuleFor(m => m.AvailabilitySpan).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly AvalancheVaultContext _context;

        public Handler(AvalancheVaultContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var ticket =
                await _context.Tickets.SingleOrDefaultAsync(
                    m => m.StoreId == request.StoreId && m.UserId == request.UserId, cancellationToken);

            if (ticket is null)
            {
                ticket = new Ticket
                {
                    UserId = request.UserId,

                    StoreId = request.StoreId,
                    
                    Validities = new List<Validity>()
                };

                await _context.Tickets.AddAsync(ticket, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var validity = new Validity
            {
                TicketId = ticket.Id,

                From = request.Availability,
                To = request.Availability.Add(request.AvailabilitySpan)
            };

            await _context.Validities.AddAsync(validity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}