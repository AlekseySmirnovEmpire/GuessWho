namespace Core.Server.Managers;

public interface IMessageBusManager
{
    public Task Send<TSend>(string channelName, TSend message);

    public Task Receive<TReceive>(string channelName, Action<TReceive> func);
}