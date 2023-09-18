namespace Avalanche.Aggregator.Application.Auth.Handlers;

public class ChallengeRate : IChallenge
{
    public Task<bool> HandleAsync(Guid userId, string correlationId)
    {
        return Task.FromResult(true);
    }
}