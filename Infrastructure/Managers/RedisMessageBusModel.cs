using Core.Models.MessageBus;
using Infrastructure.Providers.MessageBus;

namespace Infrastructure.Managers;

public class RedisMessageBusModel : IMessageBusModel
{
    public string ServiceName => nameof(RedisMessageBusProvider);
    public string Data { get; set; }
    public string ChannelName { get; set; }
}