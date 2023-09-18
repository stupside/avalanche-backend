using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Licence.Application.Features;

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
        private readonly AvalancheLicenceContext _context;

        public Handler(AvalancheLicenceContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var tickets = await _context.Validities
                .Where(m => m.UserId == request.UserId)
                .GroupBy(m => m.OrganizationId)
                .Select(m =>
                    new Response(m.Key, m.Any(r => r.From <= DateTimeOffset.UtcNow && DateTimeOffset.UtcNow <= r.To)))
                .ToListAsync(cancellationToken);

            return tickets;
        }
    }

    [Serializable]
    public record Response(Guid OrganizationId, bool IsValid);
}