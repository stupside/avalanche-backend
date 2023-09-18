using Avalanche.Corporation.Application.Domain;
using Avalanche.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Corporation.Application.Features.Organizations;

public static class CreateOrganization
{
    public sealed record Command(Guid UserId, string Name) : IRequest<Guid>;
    
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.UserId).NotEmpty();

            RuleFor(m => m.Name).NotEmpty()
                .MinimumLength(Organization.NameMinLenght)
                .MaximumLength(Organization.NameMaxLenght);
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
            var exists = await _context.Organizations.AnyAsync(m => m.Name == request.Name, cancellationToken);

            if (exists)
                throw new BusinessRuleException("Organization name already taken");
            
            var organization = new Organization
            {
                UserId = request.UserId,
                Name = request.Name
            };

            var entry = await _context.Organizations.AddAsync(organization, cancellationToken);

            return entry.Entity.Id;
        }
    }
}