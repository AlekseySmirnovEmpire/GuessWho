using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var response = await httpClient.GetAsync("appsettings.json");

// Создаем конфигурацию
var configs = new ConfigurationBuilder()
    .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(await response.Content.ReadAsStringAsync())))
    .Build();

builder.Configuration.AddConfiguration(configs);
var c = configs["ApiUrl"];
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(configs["ApiUrl"] ?? builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<CustomHttpClient>();
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    config.JsonSerializerOptions.WriteIndented = false;
});
builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();