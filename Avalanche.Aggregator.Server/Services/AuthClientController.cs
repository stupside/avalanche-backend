using Avalanche.Aggregator.Application;
using Avalanche.Aggregator.Application.Auth;
using Avalanche.Aggregator.Application.Auth.Pipeline;
using Avalanche.Aggregator.Application.Domain;
using Avalanche.Exceptions;
using Avalanche.Identity.Client;
using Avalanche.Ticket.Validity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Aggregator.Server.Services;

[Controller]
public class AuthClientController : ControllerBase
{
    private readonly AvalancheAggregatorContext _context;
    private readonly ChallengeManager _manager;

    private readonly ValidityService.ValidityServiceClient _validity;

    private readonly IEnumerable<IChallenge> _challenges;

    public AuthClientController(AvalancheAggregatorContext context, ChallengeManager manager,
        ValidityService.ValidityServiceClient validity, IEnumerable<IChallenge> challenges)
    {
        _manager = manager;
        _validity = validity;
        _challenges = challenges;
        _context = context;
    }

    [HttpPost]
    public async Task Authenticate(Guid recordId, string correlationId, HttpContext context)
    {
        var record = await _context.Records.SingleOrDefaultAsync(m => m.Id == recordId);

        if (record is null)
            throw new NotFoundException(typeof(Record), recordId);

        var notifier = _manager.Get(record.ServerId);

        var server = await _context.Servers.SingleOrDefaultAsync(m => m.Id == record.ServerId);

        if (server is null)
            throw new NullReferenceException();

        var user = context.GetUserId();
        var organisation = server.OrganizationId.ToString();

        var ticket = await _validity.ChallengeAsync(new Challenge.Types.Request
        {
            OrganizationId = organisation,
            UserId = user.ToString()
        });

        if (ticket.IsValid)
        {
            foreach (var challenge in _challenges)
            {
                if (await challenge.HandleAsync(user, correlationId))
                    continue;

                await notifier.NotifyChallengeAsync(
                    new ChallengeNotifier.ChallengeContext(record.Id, correlationId, user, false,
                        challenge.GetType().Name));
            }

            await notifier.NotifyChallengeAsync(new ChallengeNotifier.ChallengeContext(record.Id, correlationId,
                user, true, "Ticket is valid"));
        }
        else
        {
            await notifier.NotifyChallengeAsync(
                new ChallengeNotifier.ChallengeContext(record.Id, correlationId, user, false, "Ticket is invalid"));
        }
    }
}