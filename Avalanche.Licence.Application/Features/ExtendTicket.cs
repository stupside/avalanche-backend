using Avalanche.Licence.Application.Domain;
using FluentValidation;
using MediatR;

namespace Avalanche.Licence.Application.Features;

public static class ExtendTicket
{
    [Serializable]
    public record Command(Guid OrganizationId, Guid UserId, DateTimeOffset Availability, TimeSpan AvailabilitySpan) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.OrganizationId).NotEmpty();

            RuleFor(m => m.Availability).NotEmpty().GreaterThanOrEqualTo(DateTimeOffset.UtcNow);
            RuleFor(m => m.AvailabilitySpan).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly AvalancheLicenceContext _context;

        public Handler(AvalancheLicenceContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var validity = new Validity
            {
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                
                From = request.Availability,
                To = request.Availability.Add(request.AvailabilitySpan),
            };

            await _context.Validities.AddAsync(validity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}