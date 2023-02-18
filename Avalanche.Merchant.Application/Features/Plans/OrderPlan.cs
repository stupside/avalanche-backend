using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using Avalanche.Merchant.Events;
using DotNetCore.CAP;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Plans;

public static class OrderPlan
{
    [Serializable]
    public record Command(Guid PlanId, Guid UserId, DateTimeOffset Availability) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.PlanId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Availability).GreaterThanOrEqualTo(DateTimeOffset.UtcNow);
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly AvalancheMerchantContext _context;

        private readonly ICapPublisher _publish;

        public Handler(AvalancheMerchantContext context, ICapPublisher publish)
        {
            _context = context;
            _publish = publish;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var plan = await _context.Plans.Select(m => new
            {
                m.Id,
                m.StoreId,
                m.AvailabilitySpan
            }).SingleOrDefaultAsync(m => m.Id == request.PlanId, cancellationToken);

            if (plan is null)
                throw new NotFoundException(typeof(Plan), request.PlanId);

            await _publish.PublishAsync(nameof(OrderCompleted), new OrderCompleted
            {
                StoreId = plan.StoreId,
                UserId = request.UserId,

                Availability = request.Availability,
                AvailabilitySpan = plan.AvailabilitySpan
            }, cancellationToken: cancellationToken);
        }
    }
}