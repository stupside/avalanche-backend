using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Stores;

public static class CreateStore
{
    [Serializable]
    public record Command(Guid UserId, string Name, string Description, string Email) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Name).NotEmpty()
                .MinimumLength(Store.NameMinLenght)
                .MaximumLength(Store.NameMaxLenght);

            RuleFor(m => m.Description).NotEmpty()
                .MinimumLength(Store.DescriptionMinLenght)
                .MaximumLength(Store.DescriptionMaxLenght);

            RuleFor(m => m.Email).NotEmpty().EmailAddress();
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
            if (await _context.Stores.AnyAsync(m => m.Name == request.Name, cancellationToken))
                throw new ValidationException("Station name already taken");

            var store = new Store
            {
                UserId = request.UserId,

                Name = request.Name,
                Description = request.Description,

                Email = request.Email
            };

            var entry = await _context.Stores.AddAsync(store, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entry.Entity.Id;
        }
    }
}