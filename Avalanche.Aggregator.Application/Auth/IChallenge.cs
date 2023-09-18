namespace Avalanche.Aggregator.Application.Auth;

public interface IChallenge
{
    public Task<bool> HandleAsync(Guid userId, string correlationId);
}