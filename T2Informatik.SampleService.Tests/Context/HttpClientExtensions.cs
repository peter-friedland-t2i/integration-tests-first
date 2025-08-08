using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions.Execution;

namespace T2Informatik.SampleService.Tests.Context;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };
    
    public static async Task PostAsync<T>(
        this HttpClient client,
        string route,
        T body
    )
    {
        var content = GetContent(route, body, "post");
        var response = await client.PostAsync(route, content);

        await EvaluateStatusCodeAsync(response, route, "POST");
    }

    public static async Task<T> GetAsync<T>(this HttpClient client, string route)
    {
        var response = await client.GetAsync(route);

        if (!response.IsSuccessStatusCode)
        {
            throw new AssertionFailedException(
                $"GET Request to {route} failed with status code {response.StatusCode}"
            );
        }

        return await DeserializeResponseAsync<T>(response, "GET", route);
    }

    public static async Task PutAsync<T>(
        this HttpClient client, string route,
        T body
    )
    {
        var content = GetContent(route, body, "put");
        var response = await client.PutAsync(route, content);

        await EvaluateStatusCodeAsync(response, route, "PUT");
    }

    private static async Task EvaluateStatusCodeAsync(
        HttpResponseMessage response,
        string route,
        string method
    )
    {
        if (
            !response.IsSuccessStatusCode
        )
        {
            var stream = await response.Content.ReadAsStringAsync();
            throw new AssertionFailedException(
                $"{method} Request to {route} failed with status code {response.StatusCode}. Message: {stream}"
            );
        }
    }

    private static StringContent? GetContent<T>(
        string route,
        T body,
        string method
    )
    {
        if (body == null)
            throw new InvalidOperationException($"{method} '{route}' with null body not allowed");

        var json = JsonSerializer.Serialize<object>(body, SerializerOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T> DeserializeResponseAsync<T>(
        HttpResponseMessage response,
        string method,
        string route
    )
    {
        var stringContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<T>(stringContent, SerializerOptions);
        if (result == null)
        {
            throw new AssertionFailedException(
                $"Cannot deserialize {method} {route} to type {typeof(T).FullName}"
            );
        }

        return result;
    }
}