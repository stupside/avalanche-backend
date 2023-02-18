namespace Avalanche.Auth.Events.Abstraction;

public sealed class TestTrailer : Dictionary<string, string>
{
    public Guid TicketId
    {
        get => Guid.Parse(this[nameof(TicketId)]);
        set => this[nameof(TicketId)] = value.ToString();
    }
}

public abstract record TestRequest(TestContext Context, TestTrailer Trailer, string Test);

public abstract record TestResult(TestContext Context, TestTrailer Trailers, string Test, bool Success);

public abstract record TestRequest<TTestRequest>(TestContext Context, TestTrailer Trailer) : TestRequest(Context, Trailer, typeof(TTestRequest).Name)
    where TTestRequest : TestRequest<TTestRequest>
{
    public sealed record Result(TestContext Context, TestTrailer Trailer, string Test, bool Success) : TestResult(Context, Trailer, Test, Success);

    public static Result ResultFor(TTestRequest request, bool success)
    {
        return new Result(request.Context, request.Trailer, typeof(TTestRequest).Name, success);
    }
}