using Avalanche.Auth.Events;
using Avalanche.Auth.Events.Abstraction;
using Avalanche.Drm.Application;
using Avalanche.Drm.Application.Domain;
using Avalanche.Drm.Application.Flow.Channels;
using Avalanche.Drm.Auth;
using Avalanche.Identity.Client;
using DotNetCore.CAP;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Avalanche.Drm.Server.Services;

[Authorize]
public class AuthService : Auth.AuthService.AuthServiceBase
{
    private readonly AvalancheDrmContext _context;
    private readonly TestChannelManager _manager;
    private readonly ICapPublisher _publisher;

    public AuthService(AvalancheDrmContext context, TestChannelManager manager, ICapPublisher publisher)
    {
        _context = context;
        _manager = manager;
        _publisher = publisher;
    }

    public override async Task Acquire(AcquireChallengeRpc.Types.Command request,
        IServerStreamWriter<AcquireChallengeRpc.Types.Response> responseStream, ServerCallContext context)
    {
        if (Guid.TryParse(request.ChallengeId, out var id) is false)
            throw new ValidationException("Invalid guid");

        var challenge = await _context.Challenges.SingleOrDefaultAsync(m => m.Id == id);

        if (challenge is null)
            throw new ArgumentException();

        var processor = _manager.GetChannelProcessor(challenge.Id);

        if (processor is null)
            return;

        var user = context.GetHttpContext().GetUserId();

        var test = new TestContext(challenge.StoreId, challenge.Id, user);

        var discover = new DrmTest.Ticket(test, new TestTrailer());

        await _publisher.PublishAsync(discover.Test, discover);

        processor.OnTestResultAsync += async result =>
        {
            var message = new AcquireChallengeRpc.Types.Response
            {
                Success = result.Success,
                Message = result.Test,
                TicketId = result.Trailers.TicketId.ToString()
            };

            await responseStream.WriteAsync(message, context.CancellationToken);
        };

        await processor.ObserveAsync(context.CancellationToken);
    }

    public override async Task<AcceptChallengeRpc.Types.Response> Accept(AcceptChallengeRpc.Types.Command request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.StoreId, out var store) is false)
            throw new ValidationException("Invalid guid");

        var challenge = new Challenge
        {
            StoreId = store
        };

        var entry = await _context.Challenges.AddAsync(challenge);

        await _context.SaveChangesAsync(context.CancellationToken);

        return new AcceptChallengeRpc.Types.Response
        {
            ChallengeId = entry.Entity.Id.ToString()
        };
    }

    public override async Task Watch(WatchChallengeRpc.Types.Command request,
        IServerStreamWriter<WatchChallengeRpc.Types.Response> responseStream, ServerCallContext context)
    {
        if (Guid.TryParse(request.ChallengeId, out var id) is false)
            throw new ValidationException("Invalid guid");

        var challenge = await _context.Challenges.SingleOrDefaultAsync(m => m.Id == id);

        if (challenge is null)
            return;
        
        var queue = new Queue<Func<TestResult, TestRequest>>();

        queue.Enqueue(m => new DrmTest.TicketRate(m.Context, m.Trailers));
        queue.Enqueue(m => new DrmTest.TicketValidity(m.Context, m.Trailers));

        var channel = _manager.GetOrAddChannel(challenge.Id, queue);
        
        channel.Processor.OnTestResultAsync += async result =>
        {
            var response = new WatchChallengeRpc.Types.Response
            {
                Success = result.Success,
                Message = result.Test,
                TicketId = result.Trailers.TicketId.ToString()
            };

            await responseStream.WriteAsync(response, context.CancellationToken);

            if (result.Success)
            {
                if (queue.TryDequeue(out var factory))
                {
                    var next = factory(result);

                    await _publisher.PublishAsync(next.Test, next, cancellationToken: context.CancellationToken);
                }
            }
        };

        await channel.Processor.ObserveAsync(context.CancellationToken);
    }
}