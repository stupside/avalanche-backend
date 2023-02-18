using Avalanche.Exceptions;
using Avalanche.Merchant.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Merchant.Application.Features.Plans;

public static class GetPlan
{
    [Serializable]
    public record Request(Guid PlanId) : IRequest<Response>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.PlanId).NotEmpty();
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
            var plan = await _context.Plans.SingleOrDefaultAsync(m => m.Id == request.PlanId, cancellationToken);

            if (plan is null) throw new NotFoundException(typeof(Plan), request.PlanId);

            return new Response
            {
                Name = plan.Name,
                Duration = plan.AvailabilitySpan,
                Price = plan.Price,
                StoreId = plan.StoreId
            };
        }
    }

    [Serializable]
    public record Response
    {
        public required string Name { get; init; }

        public required uint Price { get; init; }

        public required TimeSpan Duration { get; init; }

        public required Guid StoreId { get; init; }
        
        public bool IsFree => Price <= 0;
    }
}