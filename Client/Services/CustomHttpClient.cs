using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Models.Auth;
using Microsoft.AspNetCore.Components;

namespace Client.Services;

public class CustomHttpClient(
    HttpClient client, 
    NavigationManager manager,
    LocalStorageService localStorage, 
    IConfiguration config)
{
    public async Task<HttpResponseMessage?> GetAsync(string requestUri, bool needHandleError = true)
    {
        try
        {
            var response = await Send(HttpMethod.Get, requestUri);
            if (needHandleError) await HandleError(response);

            return response;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<HttpResponseMessage> PostAsync<TIn>(string requestUri, TIn? body)
    {
        var bodyReq = body == null 
            ? null 
            : new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        return await Send(HttpMethod.Post, requestUri, bodyReq);
    }
    
    public async Task<HttpResponseMessage> PutAsync<TIn>(string requestUri, TIn? body)
    {
        var bodyReq = body == null 
            ? null 
            : new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        return await Send(HttpMethod.Put, requestUri, bodyReq);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri) => await Send(HttpMethod.Delete, requestUri);

    private async Task AddAuth()
    {
        try
        {
            var tokenModel = await localStorage.GetItemAsync<TokenModel>("Guess_Who");
            if (string.IsNullOrEmpty(tokenModel?.Access)) return;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel.Access);
        }
        catch
        {
            //
        }
    }

    private async Task RefreshAuth()
    {
        try
        {
            var tokenModel = await localStorage.GetItemAsync<TokenModel>("Guess_Who");
            if (string.IsNullOrEmpty(tokenModel?.Refresh)) return;

            var response = await client.PostAsync(
                $"{config["ApiUrl"]}/api/v1/auth/refresh",
                new StringContent(JsonSerializer.Serialize(tokenModel), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                await localStorage.SetItemAsync("Guess_Who", new object());

                return;
            }
            
            var newTokenModel = await JsonSerializer.DeserializeAsync<TokenModel>(
                await response.Content.ReadAsStreamAsync());
            if (string.IsNullOrEmpty(newTokenModel?.Access)) return;

            await localStorage.SetItemAsync("Guess_Who", newTokenModel);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newTokenModel.Access);
        }
        catch
        {
            //
        }
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string requestUri, StringContent? body = null)
    {
        await AddAuth();
        var request = new HttpRequestMessage(method, $"{config["ApiUrl"]}{requestUri}");
        if (body != null) request.Content = body;

        var response = await client.SendAsync(request);
        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        await RefreshAuth();
        var token = await localStorage.GetItemAsync<TokenModel>("Guess_Who");
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = client.BaseAddress;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Access);
        var newRequest = new HttpRequestMessage(method, $"{config["ApiUrl"]}{requestUri}");
        if (body != null) request.Content = body;

        return await httpClient.SendAsync(newRequest);
    }

    private async Task HandleError(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            manager.NavigateTo("/error/403");
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            manager.NavigateTo("/error/404");
        }
        else if (!response.IsSuccessStatusCode)
        {
            // Обработка других ошибок (500, 404 и т. д.)
            manager.NavigateTo("/error");
        }
    }
}