using System.Collections.Concurrent;
using System.Threading.Channels;
using Avalanche.Aggregator.Application.Auth.Pipeline;

namespace Avalanche.Aggregator.Application.Auth;

public class ChallengeManager
{
    private readonly ConcurrentDictionary<Guid, Channel<ChallengeNotifier.ChallengeContext>> _channels = new();

    public ChallengeProcessor Add(Guid server)
    {
        var channel = Channel.CreateUnbounded<ChallengeNotifier.ChallengeContext>();

        if(_channels.TryAdd(server, channel))
            return new ChallengeProcessor(channel);

        throw new InvalidOperationException("Server already registered");
    }

    public ChallengeNotifier Get(Guid server)
    {
        if (_channels.TryGetValue(server, out var channel))
            return new ChallengeNotifier(channel);

        throw new NullReferenceException("Server is not connected");
    }

    public void Remove(Guid server)
    {
        if (_channels.TryGetValue(server, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }
}