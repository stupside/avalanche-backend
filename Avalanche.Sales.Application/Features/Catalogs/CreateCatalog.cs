using Avalanche.Sales.Application.Domain;
using FluentValidation;
using MediatR;

namespace Avalanche.Sales.Application.Features.Catalogs;

public static class CreateCatalog
{
    [Serializable]
    public record Command(Guid OrganizationId) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.OrganizationId).NotEmpty();
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
            var store = new Catalog
            {
                OrganizationId = request.OrganizationId
            };

            var entry = await _context.Catalogs.AddAsync(store, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entry.Entity.Id;
        }
    }
}