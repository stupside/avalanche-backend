using System.Threading.Channels;
using Avalanche.Auth.Events.Abstraction;

namespace Avalanche.Drm.Application.Flow.Channels;

public sealed class TestChannel : Channel<TestResult>
{
   
    public TestChannelProcessor Processor { get; }

    public TestChannel(Queue<Func<TestResult, TestRequest>> queue)
    {
        var options = new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,

            SingleReader = false,
            SingleWriter = false
        };

        var channel = Channel.CreateUnbounded<TestResult>(options);

        Writer = channel.Writer;
        Reader = channel.Reader;

        Processor = TestChannelProcessor.ForChannel(this, queue);
    }
}

public sealed class TestChannelProcessor
{
    private readonly ChannelReader<TestResult> _reader;

    private int _tests;
    
    private TestChannelProcessor(ChannelReader<TestResult> reader, int tests)
    {
        _reader = reader;
        _tests = tests;
    }

    public static TestChannelProcessor ForChannel(TestChannel channel, Queue<Func<TestResult, TestRequest>> queue)
    {
        return new TestChannelProcessor(channel.Reader, queue.Count + 1);
    }

    public event Func<TestResult, Task>? OnTestResultAsync;

    public async Task ObserveAsync(CancellationToken cancellationToken = new ())
    {
        if (OnTestResultAsync is null)
            throw new NotSupportedException();

        var enumerable = _reader.ReadAllAsync(cancellationToken);

        await foreach (var result in enumerable.WithCancellation(cancellationToken))
        {
            await OnTestResultAsync(result);
            
            if (result.Success is false)
                break;

            if(--_tests <= 0)
                break;
        }
    }
}