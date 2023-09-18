using Avalanche.Exceptions;
using Avalanche.Sales.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Sales.Application.Features.Plans;

public static class DeletePlan
{
    [Serializable]
    public record Command(Guid PlanId) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.PlanId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly SalesContext _context;

        public Handler(SalesContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var plan = await _context.Plans.SingleOrDefaultAsync(
                m => m.Id == request.PlanId, cancellationToken: cancellationToken);

            if (plan is null) throw new NotFoundException(typeof(Plan), request.PlanId);
            
            _context.Remove(plan);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}