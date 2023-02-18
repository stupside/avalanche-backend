using Avalanche.Auth.Events;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Vault.Application.Consumers;

public class ChallengerConsumer : ICapSubscribe
{
    private readonly AvalancheVaultContext _context;

    private readonly ICapPublisher _publisher;

    public ChallengerConsumer(ICapPublisher publisher, AvalancheVaultContext context)
    {
        _publisher = publisher;
        _context = context;
    }

    [CapSubscribe(nameof(DrmTest.Ticket))]
    public async Task Consume(DrmTest.Ticket test)
    {
        var ticket =
            await _context.Tickets.SingleOrDefaultAsync(m =>
                m.StoreId == test.Context.StoreId && m.UserId == test.Context.UserId);

        if (ticket is null)
        {
            var result = DrmTest.Ticket.ResultFor(test, false);

            await _publisher.PublishAsync(nameof(DrmTest.Ticket.Result), result);
        }
        else
        {
            var result = DrmTest.Ticket.ResultFor(test, true);

            result.Trailers.TicketId = ticket.Id;

            await _publisher.PublishAsync(nameof(DrmTest.Ticket.Result), result);
        }
    }

    [CapSubscribe(nameof(DrmTest.TicketRate))]
    public async Task Consume(DrmTest.TicketRate test)
    {
        var result = DrmTest.TicketRate.ResultFor(test, true);

        await _publisher.PublishAsync(nameof(DrmTest.TicketRate.Result), result);
    }

    [CapSubscribe(nameof(DrmTest.TicketValidity))]
    public async Task Consume(DrmTest.TicketValidity test)
    {
        var now = DateTimeOffset.UtcNow;

        var validity = await _context.Validities
            .AnyAsync(m => m.TicketId == test.Trailer.TicketId && m.From <= now && now <= m.To);

        var result = DrmTest.TicketValidity.ResultFor(test, validity);

        await _publisher.PublishAsync(nameof(DrmTest.TicketValidity.Result), result);
    }
}