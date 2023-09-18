using Avalanche.Corporation.Application.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Corporation.Application.Features.Invitations;

public static class SendInvitation
{
    public sealed record Command(Guid OrganizationId, string Email) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.OrganizationId).NotEmpty();

            RuleFor(m => m.Email).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Guid>
    {
        private readonly CorporationContext _context;

        public Handler(CorporationContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var invitation = await _context.Invitations.SingleOrDefaultAsync(m =>
                m.OrganizationId == request.OrganizationId && m.Email == request.Email, cancellationToken);

            if (invitation is null)
            {
                invitation = new Invitation
                {
                    Email = request.Email,
                    OrganizationId = request.OrganizationId,
                    Status = Invitation.InvitationStatus.Pending
                };

                var entry = await _context.Invitations.AddAsync(invitation, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                // TODO: Send Email

                return entry.Entity.Id;
            }

            throw new NotImplementedException();
        }
    }
}