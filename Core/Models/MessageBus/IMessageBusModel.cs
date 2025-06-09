using System.Text.Json.Serialization;

namespace Core.Models.MessageBus;

public interface IMessageBusModel
{
    [JsonPropertyName("serviceName")] public string ServiceName { get; }

    [JsonPropertyName("data")] public string? Data { get; set; }

    [JsonPropertyName("channelName")] public string ChannelName { get; set; }
}