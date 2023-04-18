using Authentication.Models;
using Authentication.Utils;
using System.Text.Json;

namespace Authentication.Services.Auth0
{
    public class UsersAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;
        private const string API_SUFFIX = "/api/v2/users";

        public UsersAPI(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }

        #region BASIC_CRUD
        public async Task<List<object>> GetUserListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<object>>(responseContent) ?? new List<object>();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task<object> GetUserAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<object>(responseContent) ?? new object();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task<object> CreateUserAsync(Auth0User auth0User)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}{API_SUFFIX}")
            {
                Content = JsonContent.Create(auth0User)
            };

            // Add the access token to the request headers
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<object>(responseContent) ?? new object();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task<object> UpdateUserAsync(string userId, object userUpdate)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}")
            {
                Content = JsonContent.Create(userUpdate)
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<object>(responseContent) ?? new object();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }
        #endregion

        #region USER_ROLES
        public async Task<List<object>> GetUserRolesAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/roles");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<object>>(responseContent) ?? new List<object>();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task AssignUserRolesAsync(string userId, List<string> roles)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/roles")
            {
                Content = JsonContent.Create(new { roles })
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }

        public async Task DeleteUserRolesAsync(string userId, List<string> roles)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/roles")
            {
                Content = JsonContent.Create(new { roles })
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }
        #endregion

        #region USER_PERMISSIONS
        public async Task<List<object>> GetUserPermissionsAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/permissions");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<object>>(responseContent) ?? new List<object>();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task AssignPermissionsToUserAsync(string userId, List<Auth0Permission> permissions)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/permissions")
            {
                Content = JsonContent.Create(new { permissions })
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }

        public async Task DeletePermissionsFromUserAsync(string userId, List<Auth0Permission> permissions)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/permissions")
            {
                Content = JsonContent.Create(new { permissions })
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }
        #endregion

        #region USER_LOGS
        public async Task<List<object>> GetUserLogsAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{userId}/logs");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<object>>(responseContent) ?? new List<object>();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }
        #endregion
    }
}