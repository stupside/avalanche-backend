using System.Threading.Channels;

namespace Avalanche.Aggregator.Application.Auth.Pipeline;

public sealed class ChallengeNotifier
{
    private readonly ChannelWriter<ChallengeContext> _writer;

    public record ChallengeContext(Guid RecordId, string CorrelationId, Guid UserId, bool Succeed, string Message);

    public ChallengeNotifier(ChannelWriter<ChallengeContext> writer)
    {
        _writer = writer;
    }

    public async Task NotifyChallengeAsync(ChallengeContext context, CancellationToken cancellationToken = new())
    {
        await _writer.WriteAsync(context, cancellationToken);
    }
}