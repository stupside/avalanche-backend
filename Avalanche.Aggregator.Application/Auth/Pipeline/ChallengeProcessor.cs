using System.Threading.Channels;

namespace Avalanche.Aggregator.Application.Auth.Pipeline;

public sealed class ChallengeProcessor
{
    private readonly ChannelReader<ChallengeNotifier.ChallengeContext> _reader;

    public ChallengeProcessor(ChannelReader<ChallengeNotifier.ChallengeContext> reader)
    {
        _reader = reader;
    }

    public async Task WaitForChallengeAsync(Func<ChallengeNotifier.ChallengeContext, Task> callback,
        CancellationToken cancellationToken = new())
    {
        var challenge = await _reader.ReadAsync(cancellationToken);

        await callback(challenge);
    }
}