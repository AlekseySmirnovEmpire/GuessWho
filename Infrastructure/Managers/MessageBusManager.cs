using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Core.Managers;
using Core.Models.MessageBus;
using Core.Providers;
using Infrastructure.Providers.MessageBus;

namespace Infrastructure.Managers;

public class MessageBusManager(IMessageBusProvider messageBusProvider) : IMessageBusManager
{
    private readonly IMessageBusModel? _messageBusModel = messageBusProvider switch
    {
        RedisMessageBusProvider => new RedisMessageBusModel(),
        _ => null
    };
    private readonly List<string> _channelsList =
    [
        JobStart
    ];
    private readonly Lock _mutex = new();

    public const string JobStart = "job-start-channel";

    public async Task Send<TSend>(string channelName, TSend message)
    {
        try
        {
            if (!_channelsList.Contains(channelName) || _messageBusModel == null)
                throw new Exception($"Канал с имеем {channelName} не найден");
            
            _messageBusModel.ChannelName = channelName;
            _messageBusModel.Data = typeof(TSend) == typeof(string) 
                ? message!.ToString() 
                : JsonSerializer.Serialize(message);
            await messageBusProvider.Send(_messageBusModel);
        }
        catch
        {
            //
        }
    }

    public async Task Receive<TReceive>(string channelName, Action<TReceive> func)
    {
        try
        {
            if (!_channelsList.Contains(channelName) || _messageBusModel == null)
                throw new Exception($"Канал с имеем {channelName} не найден");

            await messageBusProvider.Receive(channelName, message =>
            {
                if (string.IsNullOrEmpty(message)) return;

                TReceive? data;
                lock (_mutex)
                {
                    data = typeof(TReceive) == typeof(string)
                        ? (TReceive)Convert.ChangeType(message, typeof(TReceive))
                        : JsonSerializer.Deserialize<TReceive>(message, new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                            WriteIndented = true
                        });
                }

                func(data!);
            });
        }
        catch
        {
            //
        }
    }
}