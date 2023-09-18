using Avalanche.Exceptions;
using Avalanche.Sales.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Sales.Application.Features.Plans;

public static class OrderPlan
{
    [Serializable]
    public record Command(Guid PlanId, Guid UserId, DateTimeOffset Availability) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.PlanId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Availability).GreaterThanOrEqualTo(DateTimeOffset.UtcNow);
        }
    }

    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly SalesContext _context;
        
        public Handler(SalesContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var plan = await _context.Plans.SingleOrDefaultAsync(m => m.Id == request.PlanId, cancellationToken);

            if (plan is null)
                throw new NotFoundException(typeof(Plan), request.PlanId);

            var order = new Order
            {
                PlanId = plan.Id,
                UserId = request.UserId,
                
                AmountDue = plan.Price,

                Availability = request.Availability,
                AvailabilitySpan = plan.AvailabilitySpan
            };

            var entry = await _context.Orders.AddAsync(order, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            // TODO: Create bill

            return entry.Entity.Id;
        }
    }
}