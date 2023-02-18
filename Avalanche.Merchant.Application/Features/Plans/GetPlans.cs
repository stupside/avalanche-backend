using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Plans;

public static class GetPlans
{
    [Serializable]
    public record Request(Guid StoreId) : IRequest<IEnumerable<Response>>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.StoreId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly AvalancheMerchantContext _context;

        public Handler(AvalancheMerchantContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var plans = await _context.Plans.Where(m => m.StoreId == request.StoreId)
                .Select(m => new Response(m.Id, m.Name, m.AvailabilitySpan, m.Price))
                .ToListAsync(cancellationToken);

            return plans;
        }
    }

    [Serializable]
    public record Response(Guid PlanId, string Name, TimeSpan Duration, uint Price)
    {
        public bool IsFree => Price <= 0;
    }
}