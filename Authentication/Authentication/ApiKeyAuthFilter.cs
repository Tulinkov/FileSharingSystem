using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http.Json;

namespace Authentication
{
    /// <summary>
    /// A filter that asynchronously confirms request authorization
    /// </summary>
    public class ApiKeyAuthFilter : IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Response format from the authentication server
        /// </summary>
        private class AuthResponse
        {
            public int statusCode { get; set; }
        }

        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        private HttpClient _client = new HttpClient();
        private readonly IConfiguration _config;

        //constuctor
        public ApiKeyAuthFilter(IConfiguration conf)
        {
            _config = conf;
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>Authorization filter context
        /// <param name="context"></param>
        /// <returns>A Task that on completion indicates the filter has executed</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //The request doesn't contain the header X-Api-Key
            if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API key missing");
                return;
            }

            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync($"https://localhost:8086/api/v1/auth/key?key={extractedApiKey}");
            } catch (Exception ex)
            {
                //The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
                context.Result = new UnauthorizedObjectResult("Error connecting to authorization server");
                return;
            }
            
            if (response.IsSuccessStatusCode)
            {
                AuthResponse? ar = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (ar?.statusCode != 200)
                {
                    //Invalid API key
                    context.Result = new UnauthorizedObjectResult("Invalid API key");
                    return;
                }
            }
            else
            {
                //The authentication server returned an error message
                context.Result = new UnauthorizedObjectResult("Error connecting to authorization server");
                return;
            }
        }
    }
}