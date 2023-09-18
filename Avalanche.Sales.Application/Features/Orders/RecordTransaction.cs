using Avalanche.Sales.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Sales.Application.Features.Orders;

public static class RecordTransaction
{
    public sealed record Command(Guid OrderId, uint AmountReceived) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.OrderId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Guid>
    {
        private readonly SalesContext _context;

        public Handler(SalesContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.Id == request.OrderId, cancellationToken);

            if (order is null)
                throw new NotImplementedException();

            var received = await _context.Transactions.SumAsync(m => m.AmountReceived, cancellationToken);

            var transaction = new Transaction
            {
                OrderId = request.OrderId,
                AmountReceived = request.AmountReceived,
            };

            var entry = await _context.Transactions.AddAsync(transaction, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            if (received + request.AmountReceived == order.AmountDue)
            {
                // TODO: order fulfilled
                // TODO: create the ticket
            }
            else if (received + request.AmountReceived > order.AmountDue)
            {
                // TODO: we received too much money
            }
            else if (received + request.AmountReceived < order.AmountDue)
            {
                // TODO: we did not received enough money
            }

            return entry.Entity.Id;
        }
    }
}