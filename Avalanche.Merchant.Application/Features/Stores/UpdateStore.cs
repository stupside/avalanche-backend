using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Stores;

public static class UpdateStore
{
    [Serializable]
    public record Command(Guid StoreId, Guid UserId, string Description, string Email) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.StoreId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Description).NotEmpty()
                .MinimumLength(Store.DescriptionMinLenght)
                .MaximumLength(Store.DescriptionMaxLenght);

            RuleFor(m => m.Email).NotEmpty().EmailAddress();
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
            var station =
                await _context.Stores.SingleOrDefaultAsync(m => m.Id == request.StoreId && m.UserId == request.UserId,
                    cancellationToken);

            if (station is null) throw new NotFoundException(typeof(Store), request.StoreId);

            station.Description = request.Description;
            station.Email = request.Email;

            _context.Update(station);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}