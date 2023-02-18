using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Stores;

public static class GetStores
{
    [Serializable]
    public record RequestByName(string NameSearch) : IRequest<IEnumerable<Response>>;

    [Serializable]
    public record RequestByIdentifiers(IEnumerable<Guid> Identifiers) : IRequest<IEnumerable<Response>>;

    public class RequestByNameValidator : AbstractValidator<RequestByName>
    {
        public RequestByNameValidator()
        {
            RuleFor(m => m.NameSearch).MinimumLength(2).NotEmpty();
        }
    }

    public class RequestByIdentifiersValidator : AbstractValidator<RequestByIdentifiers>
    {
        public RequestByIdentifiersValidator()
        {
            RuleFor(m => m.Identifiers).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<RequestByName, IEnumerable<Response>>,
        IRequestHandler<RequestByIdentifiers, IEnumerable<Response>>
    {
        private readonly AvalancheMerchantContext _context;

        public Handler(AvalancheMerchantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Response>> Handle(RequestByName request, CancellationToken cancellationToken)
        {
            var stations = await _context.Stores
                .Where(m => m.Name.ToLower().Contains(request.NameSearch.ToLower()))
                .Select(m => new Response(m.Id, m.Name, m.Description, m.Logo))
                .ToListAsync(cancellationToken);

            return stations;
        }

        public async Task<IEnumerable<Response>> Handle(RequestByIdentifiers request,
            CancellationToken cancellationToken)
        {
            var stations = await _context.Stores
                .Where(m => request.Identifiers.Contains(m.Id))
                .Select(m => new Response(m.Id, m.Name, m.Description, m.Logo))
                .ToListAsync(cancellationToken);

            return stations;
        }
    }

    [Serializable]
    public record Response(Guid StoreId, string Name, string Description, string? Logo);
}