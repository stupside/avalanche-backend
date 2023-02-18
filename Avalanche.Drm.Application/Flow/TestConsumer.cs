using Avalanche.Auth.Events;
using Avalanche.Auth.Events.Abstraction;
using Avalanche.Drm.Application.Flow.Channels;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Drm.Application.Flow;

public class TestConsumer : ICapSubscribe
{
    private readonly AvalancheDrmContext _context;

    private readonly TestChannelManager _manager;

    public TestConsumer(AvalancheDrmContext context, TestChannelManager manager)
    {
        _context = context;
        _manager = manager;
    }

    [CapSubscribe(nameof(DrmTest.Ticket.Result))]
    public async Task Consume(DrmTest.Ticket.Result result)
    {
        await HandleAsync(result);
    }

    [CapSubscribe(nameof(DrmTest.TicketRate.Result))]
    public async Task Consume(DrmTest.TicketRate.Result result)
    {
        await HandleAsync(result);
    }


    [CapSubscribe(nameof(DrmTest.TicketValidity.Result))]
    public async Task Consume(DrmTest.TicketValidity.Result result)
    {
        await HandleAsync(result);
    }


    private async Task HandleAsync(TestResult result)
    {
        var challenge = await _context.Challenges.SingleOrDefaultAsync(m => m.Id == result.Context.ChallengeId);

        if (challenge is null)
            return;

        var writer = _manager.GetChannelWriter(challenge.Id);

        if (writer is not null)
        {
            await writer.WriteAsync(result);
        }
    }
}