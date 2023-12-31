﻿using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoreAPI.Services;

public class OwnershipServiceClient : IOwnershipServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IBaseClientService _baseClient;
    private readonly ILogger<OwnershipServiceClient> _logger;


    // TODO: Implement BaseClient

    public OwnershipServiceClient(IHttpContextAccessor contextAccessor, ILogger<OwnershipServiceClient> logger, IBaseClientService baseClient)
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7012/"); // Replace with the base URL of the ownership service
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/ownership/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/ownership/");
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _contextAccessor = contextAccessor;
        _logger = logger;
        _baseClient = baseClient;
    }

    public async Task<JsonDocument?> GetOwnershipByIdAsync(Guid id, string? type)
    {
        string url = $"ownership/{id}";
        if (!string.IsNullOrEmpty(type)) url += $"/{type.ToLowerInvariant()}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<JsonDocument>(content); /*JsonSerializer.Deserialize<string>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });*/
        }


        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to get ownership by ID. Status code: {response.StatusCode}");
    }

    public async Task<JsonDocument?> GetOwnershipByPropertyIdAsync(Guid propertyId)
    {
        string url = $"ownership/{propertyId}/property";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<JsonDocument>(content); /*JsonSerializer.Deserialize<string>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });*/
        }


        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to get ownership by ID. Status code: {response.StatusCode}");
    }

    public async Task<JsonDocument?> CreateOwnershipAsync(string requestBody)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "ownership");

        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();


        if (response.IsSuccessStatusCode)
        {
            return string.IsNullOrEmpty(content) ? null : JsonSerializer.Deserialize<JsonDocument>(content);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to create ownership. Status code: {response.StatusCode}. {content}");
    }

    public async Task<JsonDocument?> CreateOwnershipsAsync(string requestBody)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "ownership");

        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return string.IsNullOrEmpty(content) ? null : JsonSerializer.Deserialize<JsonDocument>(content);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to create ownerships. Status code: {response.StatusCode}. {content}");
    }

    public async Task DeleteOwnershipsAsync(Guid propertyId)
    {

        _logger.LogInformation($"Deleting ownership information for property {propertyId}");

        JsonDocument? ownerships = await _baseClient.ReadAsync($"ownership/ownership/{propertyId}/property");

        if (ownerships is null || ownerships.RootElement.GetArrayLength() == 0)
        {
            _logger.LogInformation($"No ownership information found for property {propertyId}");
            return;
        }

        // TODO: Create delete ownerships by propertyId endpoint in OwnershipsAPI?
        foreach (var ownership in ownerships.RootElement.EnumerateArray())
        {
            await _baseClient.DeleteAsync($"ownership/ownership/{ownership.GetProperty("id").GetString()}");
        }

        _logger.LogInformation($"Successfully deleted ownership information for property {propertyId}");
        return;
    }

}