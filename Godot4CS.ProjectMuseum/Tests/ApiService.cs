using System;
using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Tests;

public class ApiService
{
    public async Task SendPostRequestAsync(string baseUrl, string endpoint, string requestData)
    {
        var httpRequest = new HttpRequest();
        string[] headers = { "Content-Type: application/json"};

        try
        {
            Error error =  httpRequest.Request($"{baseUrl}/{endpoint}", headers, Godot.HttpClient.Method.Post, requestData);
        }
        catch (TaskCanceledException)
        {
            throw new Exception("Request timed out");
        }
        
    }

    // public async Task<string> SendGetRequestAsync(string baseUrl, string endpoint, int timeoutSeconds = 30)
    // {
    //     using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync($"{baseUrl}/{endpoint}", cts.Token);
    //
    //             if (response.IsSuccessStatusCode)
    //             {
    //                 return await response.Content.ReadAsStringAsync();
    //             }
    //             else
    //             {
    //                 throw new Exception($"GET request failed with status code: {response.StatusCode}");
    //             }
    //         }
    //         catch (TaskCanceledException)
    //         {
    //             throw new Exception("Request timed out");
    //         }
    //     }
    // }
}