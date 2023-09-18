using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Corporation.Application.Features.Members;

public static class RemoveMember
{
    public sealed record Command(Guid MemberId) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.MemberId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly CorporationContext _context;

        public Handler(CorporationContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var member = await _context.Members.SingleOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);

            if (member is not null)
            {
                _context.Members.Remove(member);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}