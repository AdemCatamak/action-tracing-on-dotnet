using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WebApplication.HttpInterceptors
{
    public class HttpClientTraceIdInterceptor : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<HttpClientTraceIdInterceptorConfiguration> _options;

        public HttpClientTraceIdInterceptor(IHttpContextAccessor httpContextAccessor, IOptions<HttpClientTraceIdInterceptorConfiguration> options = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string traceId = _httpContextAccessor.HttpContext.TraceIdentifier;

            HttpClientTraceIdInterceptorConfiguration httpClientTraceIdInterceptorConfiguration = _options?.Value ?? new HttpClientTraceIdInterceptorConfiguration();
            if (!string.IsNullOrEmpty(traceId)
             && request.Headers.All(h => h.Key != httpClientTraceIdInterceptorConfiguration.TraceIdHeader))
            {
                request.Headers.Add(httpClientTraceIdInterceptorConfiguration.TraceIdHeader, traceId);
            }

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            return httpResponseMessage;
        }

        public class HttpClientTraceIdInterceptorConfiguration
        {
            public const string DefaultTraceIdHeader = "x-trace-id";
            public string TraceIdHeader { get; set; } = DefaultTraceIdHeader;
        }
    }
}