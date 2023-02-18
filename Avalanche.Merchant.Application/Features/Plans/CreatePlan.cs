using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Plans;

public static class CreatePlan
{
    [Serializable]
    public record Command
        (Guid StoreId, Guid UserId, string Name, TimeSpan Validity, uint Price) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.StoreId).NotEmpty();

            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Name).NotEmpty()
                .MinimumLength(Plan.NameMinLenght)
                .MaximumLength(Plan.NameMaxLenght);

            RuleFor(m => m.Validity).GreaterThanOrEqualTo(TimeSpan.FromMinutes(30));
            RuleFor(m => m.Price).GreaterThanOrEqualTo(100U);
        }
    }

    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly AvalancheMerchantContext _context;

        public Handler(AvalancheMerchantContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var store = await _context.Stores.SingleOrDefaultAsync(
                m => m.Id == request.StoreId && m.UserId == request.UserId, cancellationToken);

            if (store is null) throw new NotFoundException(typeof(Store), request.StoreId);

            if (await _context.Plans.AnyAsync(m => m.Name == request.Name && m.StoreId == store.Id, cancellationToken))
                throw new BusinessRuleException("Plan name already taken");

            var plan = new Plan
            {
                StoreId = request.StoreId,
                Name = request.Name,
                AvailabilitySpan = request.Validity,
                Price = request.Price
            };

            var entry = await _context.AddAsync(plan, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entry.Entity.Id;
        }
    }
}