using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CoreAPI.Services
{
    public abstract class BaseClientService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _contextAccessor;

        protected BaseClientService(IHttpContextAccessor contextAccessor, string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _contextAccessor = contextAccessor;
        }

        protected HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
        {
            var request = new HttpRequestMessage(method, endpoint);
            var auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            if (auth != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Split(' ')[1]);
            }
            return request;
        }

        protected async Task<string?> ReadAsync<T>(string endpoint)
        {
            var request = CreateRequest(HttpMethod.Get, endpoint);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            // Handle other status codes as needed
            return default;
        }

        protected async Task<string?> CreateAsync<T>(string endpoint, T data)
        {
            var request = CreateRequest(HttpMethod.Post, endpoint);
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            // Handle other status codes as needed
            return default;
        }

        protected async Task<bool> UpdateAsync<T>(string endpoint, T data)
        {
            var request = CreateRequest(HttpMethod.Put, endpoint);
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            var request = CreateRequest(HttpMethod.Delete, endpoint);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
