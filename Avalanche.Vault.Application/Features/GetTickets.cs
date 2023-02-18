using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Vault.Application.Features;

public static class GetTickets
{
    [Serializable]
    public record Request(Guid UserId) : IRequest<IEnumerable<Response>>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly AvalancheVaultContext _context;

        public Handler(AvalancheVaultContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var tickets = await _context.Tickets
                .Include(m => m.Validities)
                .Where(m => m.UserId == request.UserId)
                .Select(m => new Response(m.Id, m.StoreId, m.Validities.Any(r => r.From <= now && now <= r.To)))
                .ToListAsync(cancellationToken);

            return tickets;
        }
    }

    [Serializable]
    public record Response(Guid TicketId, Guid StoreId, bool IsValid);
}