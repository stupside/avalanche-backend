using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Stores;

public static class GetStore
{
    [Serializable]
    public record Request(Guid StoreId) : IRequest<Response>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.StoreId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly AvalancheMerchantContext _context;

        public Handler(AvalancheMerchantContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var station =
                await _context.Stores.SingleOrDefaultAsync(m => m.Id == request.StoreId, cancellationToken);

            if (station is null) throw new NotFoundException(typeof(Store), request.StoreId);

            return new Response
            {
                Name = station.Name,
                Description = station.Description,
                Email = station.Email,
                Logo = station.Logo
            };
        }
    }

    [Serializable]
    public class Response
    {
        public required string Name { get; init; }
        public required string Description { get; init; }

        public required string Email { get; init; }

        public string? Logo { get; init; }
    }
}