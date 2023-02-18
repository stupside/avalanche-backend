using Avalanche.Auth.Events.Abstraction;

namespace Avalanche.Auth.Events;

public static class DrmTest
{
    [Serializable]
    public sealed record Ticket(TestContext Context, TestTrailer Trailer) : TestRequest<Ticket>(Context, Trailer);

    [Serializable]
    public sealed record TicketRate(TestContext Context, TestTrailer Trailer) : TestRequest<TicketRate>(Context, Trailer);

    [Serializable]
    public sealed record TicketValidity(TestContext Context, TestTrailer Trailer) : TestRequest<TicketValidity>(Context, Trailer);
}