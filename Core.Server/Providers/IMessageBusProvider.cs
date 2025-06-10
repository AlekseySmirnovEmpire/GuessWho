using Core.Models.MessageBus;

namespace Core.Server.Providers;

public interface IMessageBusProvider
{
    public Task Send(IMessageBusModel model);

    public Task Receive(string channelName, Action<string> func);
}