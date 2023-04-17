using Authentication.Models;
using Authentication.Utils;
using System.Text.Json;

namespace Authentication.Services.Auth0
{
    public class RolesAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;
        private const string API_SUFFIX = "/api/v2/roles";

        public RolesAPI(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }

        #region BASIC_CRUD
        public async Task<List<object>> GetRolesListAsync()
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

        public async Task<object> GetRoleAsync(string roleId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<object>(responseContent) ?? new object();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task<object> CreateRoleAsync(Auth0Role auth0Role)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}{API_SUFFIX}")
            {
                Content = JsonContent.Create(auth0Role)
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

        public async Task<object> UpdateRoleAsync(string roleId, object roleUpdate)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}")
            {
                Content = JsonContent.Create(roleUpdate)
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

        public async Task DeleteRoleAsync(string roleId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, responseContent);
            }
        }
        #endregion

        #region ROLE_PERMISSIONS
        public async Task<List<object>> GetRolePermissionsAsync(string roleId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}/permissions");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<object>>(responseContent) ?? new List<object>();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }

        public async Task AssignPermissionsToRoleAsync(string roleId, List<Auth0Permission> permissions)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}/permissions")
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

        public async Task DeletePermissionsFromRoleAsync(string roleId, List<Auth0Permission> permissions)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}{API_SUFFIX}/{roleId}/permissions")
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
    }
}