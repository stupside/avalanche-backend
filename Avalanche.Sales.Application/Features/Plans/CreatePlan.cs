using Avalanche.Exceptions;
using Avalanche.Sales.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Sales.Application.Features.Plans;

public static class CreatePlan
{
    [Serializable]
    public record Command(Guid CatalogId, string Name, TimeSpan Validity, uint Price) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.CatalogId).NotEmpty();

            RuleFor(m => m.Name).NotEmpty()
                .MinimumLength(Plan.NameMinLenght)
                .MaximumLength(Plan.NameMaxLenght);

            RuleFor(m => m.Validity).GreaterThanOrEqualTo(TimeSpan.FromMinutes(30));
            RuleFor(m => m.Price).GreaterThanOrEqualTo(100U);
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
            var store = await _context.Catalogs.SingleOrDefaultAsync(m => m.Id == request.CatalogId, cancellationToken);

            if (store is null) throw new NotFoundException(typeof(Catalog), request.CatalogId);

            if (await _context.Plans.AnyAsync(m => m.Name == request.Name && m.CatalogId == store.Id, cancellationToken))
                throw new BusinessRuleException("Plan name already taken");

            var plan = new Plan
            {
                CatalogId = request.CatalogId,

                Name = request.Name,
                Price = request.Price,

                AvailabilitySpan = request.Validity
            };

            var entry = await _context.AddAsync(plan, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entry.Entity.Id;
        }
    }
}