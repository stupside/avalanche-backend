using System.Collections.Concurrent;
using System.Threading.Channels;
using Avalanche.Auth.Events.Abstraction;

namespace Avalanche.Drm.Application.Flow.Channels;

public class TestChannelManager
{
    private readonly ConcurrentDictionary<Guid, TestChannel> _channels;

    public TestChannelManager()
    {
        _channels = new ConcurrentDictionary<Guid, TestChannel>();
    }

    public TestChannelProcessor? GetChannelProcessor(Guid channel)
    {
        _channels.TryGetValue(channel, out var instance);

        return instance?.Processor;
    }
    
    public ChannelWriter<TestResult>? GetChannelWriter(Guid channel)
    {
        _channels.TryGetValue(channel, out var instance);

        return instance?.Writer;
    }

    public TestChannel GetOrAddChannel(Guid challenge, Queue<Func<TestResult, TestRequest>> queue)
    {
        var channel = new TestChannel(queue);
        
        return _channels.GetOrAdd(challenge, channel);
    }
}