namespace Avalanche.Auth.Events.Abstraction;

public sealed record TestContext(Guid StoreId, Guid ChallengeId, Guid UserId);