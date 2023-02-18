namespace Avalanche.Vault.Application.Domain.ValueTypes;

[Serializable]
public readonly struct TimeSpanStatus
{
    public TimeSpanStatus(DateTimeOffset from, DateTimeOffset to)
    {
        From = from;
        To = to;
    }

    public enum StatusKind
    {
        Early = 0,
        Valid = 1,
        Late = 2
    }

    public DateTimeOffset From { get; }
    public DateTimeOffset To { get; }

    public TimeSpan Span
    {
        get
        {
            var now = DateTimeOffset.UtcNow;

            if (Kind == StatusKind.Early)
                return From - now;

            if (Kind == StatusKind.Late)
                return now - To;

            return To - now;
        }
    }

    public StatusKind Kind => GetKind(From, To);

    public static StatusKind GetKind(DateTimeOffset from, DateTimeOffset to)
    {
        var now = DateTimeOffset.UtcNow;

        if (now <= from)
            return StatusKind.Early;

        if (now >= to)
            return StatusKind.Late;

        return StatusKind.Valid;
    }
}