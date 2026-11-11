using System.Net.Http.Headers;

namespace NexusRetail.WebMvc.Extensions
{
    /// <summary>
    /// DelegatingHandler tự động đính kèm JWT Access Token từ Cookie Claims vào mọi request gọi Core API.
    /// </summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null)
            {
                var token = _httpContextAccessor.HttpContext?.User.FindFirst("JwtToken")?.Value;
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

