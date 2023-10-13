using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Godot4CS.ProjectMuseum.Tests;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> SendPostRequestAsync(string baseUrl, string endpoint, object requestData, int timeoutSeconds = 30)
    {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/{endpoint}", requestData, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"POST request failed with status code: {response.StatusCode}");
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Request timed out");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Request failed with error: {ex.Message}");
            }
        }
    }

    public async Task<string> SendGetRequestAsync(string baseUrl, string endpoint, int timeoutSeconds = 30)
    {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/{endpoint}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"GET request failed with status code: {response.StatusCode}");
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Request timed out");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Request failed with error: {ex.Message}");
            }
        }
    }
}