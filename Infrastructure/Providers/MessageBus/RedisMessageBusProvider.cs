using Core.Models.MessageBus;
using Core.Server.Providers;
using StackExchange.Redis;

namespace Infrastructure.Providers.MessageBus;

public class RedisMessageBusProvider : IMessageBusProvider
{
    private readonly ConnectionMultiplexer? _connection;

    public RedisMessageBusProvider()
    {
        try
        {
            _connection = ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("ASPNETCORE_REDIS_CONNECTION_STRING") ?? string.Empty);
        }
        catch
        {
            //
        }
    }

    public async Task Send(IMessageBusModel model)
    {
        if (_connection is null)
            return;

        var bus = _connection.GetSubscriber();
        await bus.PublishAsync(
            RedisChannel.Literal(model.ChannelName),
            model.Data);
    }

    public async Task Receive(string channelName, Action<string> func)
    {
        if (_connection is null)
            return;

        var bus = _connection.GetSubscriber();
        await bus.SubscribeAsync(
            RedisChannel.Literal(channelName),
            (_, message) => { func(message!); });
    }
}