using Avalanche.Corporation.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Corporation.Application.Features.Invitations;

public static class RejectInvitation
{
    public sealed record Command(Guid InvitationId) : IRequest;

    public sealed class Validator : AbstractValidator<AcceptInvitation.Command>
    {
        public Validator()
        {
            RuleFor(m => m.InvitationId).NotEmpty();
            RuleFor(m => m.UserId).NotEmpty();
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
            var invitation =
                await _context.Invitations.SingleOrDefaultAsync(m => m.Id == request.InvitationId, cancellationToken);

            if (invitation is null)
                return;

            switch (invitation.Status)
            {
                case Invitation.InvitationStatus.Pending:
                {
                    invitation.Status = Invitation.InvitationStatus.Rejected;

                    _context.Invitations.Update(invitation);

                    await _context.SaveChangesAsync(cancellationToken);
                }
                    break;
                case Invitation.InvitationStatus.Accepted:
                case Invitation.InvitationStatus.Rejected:
                default:
                    throw new NotImplementedException();
            }
        }
    }    
}