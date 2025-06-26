using System.Text.Json;
using Microsoft.JSInterop;

namespace Client.Services;

public class LocalStorageService(IJSRuntime jsRuntime)
{
    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await jsRuntime.InvokeAsync<string>("getFromLocalStorage", key);

            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("setToLocalStorage", key, JsonSerializer.Serialize(value));
    }
}