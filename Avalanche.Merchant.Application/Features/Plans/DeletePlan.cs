using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Plans;

public static class DeletePlan
{
    [Serializable]
    public record Command(Guid PlanId, Guid UserId) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.PlanId).NotEmpty();

            RuleFor(m => m.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly AvalancheMerchantContext _context;

        public Handler(AvalancheMerchantContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var plan = await _context.Plans.SingleOrDefaultAsync(
                m => m.Id == request.PlanId, cancellationToken: cancellationToken);

            if (plan is null) throw new NotFoundException(typeof(Plan), request.PlanId);

            var store = await _context.Stores.SingleOrDefaultAsync(
                m => m.Id == plan.StoreId && m.UserId == request.UserId, cancellationToken);

            if (store is null) throw new NotFoundException(typeof(Store), plan.StoreId);

            _context.Remove(plan);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}