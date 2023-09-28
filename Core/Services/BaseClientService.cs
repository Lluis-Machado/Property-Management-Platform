using MassTransit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class BaseClientService : IBaseClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<BaseClientService> _logger;

        public BaseClientService(IHttpContextAccessor contextAccessor, ILogger<BaseClientService> logger)
        {
            _httpClient = new HttpClient();
#if DEVELOPMENT
            _httpClient.BaseAddress = new Uri("https://localhost:7142/"); // API Gateway URL
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/");
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/");
#endif
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
        {

            _logger.LogInformation($"BaseClientService - Creating new {method.Method} request towards {_httpClient.BaseAddress}{endpoint}");

            var request = new HttpRequestMessage(method, endpoint);
            var auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            if (auth != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Split(' ')[1]);
            }
            return request;
        }

        public async Task<JsonDocument?> ReadAsync(string endpoint)
        {
            var request = CreateRequest(HttpMethod.Get, endpoint);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                throw new Exception($"Request unsuccessful - {content}");
            }
            else
            {
                try
                {
                    return JsonSerializer.Deserialize<JsonDocument>(content);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"BaseClientService/ReadAsync - Error deserializing response\n{ex.Message} - {ex.StackTrace}");
                    return default;
                }
            }
        }

        public async Task<JsonDocument?> CreateAsync<T>(string endpoint, T data)
        {
            var request = CreateRequest(HttpMethod.Post, endpoint);
            string dataToSend = (typeof(T) == typeof(string) ? data.ToString() : JsonSerializer.Serialize(data)) ?? "";
            request.Content = new StringContent(dataToSend, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                throw new Exception($"Request unsuccessful - {content}");
            }
            else
            {
                try
                {
                    return JsonSerializer.Deserialize<JsonDocument>(content);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"BaseClientService/CreateAsync - Error deserializing response\n{ex.Message} - {ex.StackTrace}");
                    return default;
                }
            }
        }

        public async Task<JsonDocument?> UpdateAsync<T>(string endpoint, T data)
        {
            var request = CreateRequest(HttpMethod.Patch, endpoint);
            string dataToSend = (typeof(T) == typeof(string) ? data.ToString() : JsonSerializer.Serialize(data)) ?? "";
            request.Content = new StringContent(dataToSend, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                throw new Exception($"Request unsuccessful - {content}");
            }
            else
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return JsonSerializer.Deserialize<JsonDocument>(@"{""status"": ""OK""}");

                    return JsonSerializer.Deserialize<JsonDocument>(content);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"BaseClientService/UpdateAsync - Error deserializing response\n{ex.Message} - {ex.StackTrace}");
                    return default;
                }
            }
        }

        public async Task<JsonDocument?> UpdateAsync(string endpoint, string? parameters = null)
        {
            var request = CreateRequest(HttpMethod.Patch, endpoint + (parameters ?? ""));
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                throw new Exception($"Request unsuccessful - {content}");
            }
            else
            {
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return JsonSerializer.Deserialize<JsonDocument>(@"{""status"": ""OK""}");

                    return JsonSerializer.Deserialize<JsonDocument>(content);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"BaseClientService/UpdateAsync - Error deserializing response\n{ex.Message} - {ex.StackTrace}");
                    return default;
                }
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var request = CreateRequest(HttpMethod.Delete, endpoint);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> UndeleteAsync(string endpoint)
        {
            var request = CreateRequest(HttpMethod.Patch, endpoint);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 500) throw new Exception($"Internal server error - {content}");
                if ((int)response.StatusCode >= 400) throw new Exception($"Bad Request - {content}");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}