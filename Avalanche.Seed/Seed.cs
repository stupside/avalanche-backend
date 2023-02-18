using OpenIddict.Client;

namespace Avalanche.Seed;

[Serializable]
public sealed class Seed
{
    public const string SeedKey = "Seed";

    [Serializable]
    public class BackendConfiguration
    {
        public required OpenIddictClientRegistration Identity { get; init; }

        public required Uri Gateway { get; init; }
    }

    public required BackendConfiguration Backend { get; init; }

    [Serializable]
    public class UsersConfiguration
    {
        public uint GenerateUsers { get; init; }
    }

    public required UsersConfiguration Users { get; init; }

    [Serializable]
    public sealed class StoresConfiguration
    {
        public uint GenerateStores { get; init; }
        public uint GenerateMaxPlans { get; init; }
        public uint GenerateMaxTickets { get; init; }
    }

    public required StoresConfiguration Stores { get; init; }
}