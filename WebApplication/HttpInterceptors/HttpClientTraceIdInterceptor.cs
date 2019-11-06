using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebApplication.TraceInfoSection;

namespace WebApplication.HttpInterceptors
{
    public class HttpClientTraceIdInterceptor : DelegatingHandler
    {
        private readonly ITraceInfoAccessor _traceInfoAccessor;
        private readonly IOptions<HttpClientTraceIdInterceptorConfiguration> _options;

        public HttpClientTraceIdInterceptor(ITraceInfoAccessor traceInfoAccessor, IOptions<HttpClientTraceIdInterceptorConfiguration> options = null)
        {
            _traceInfoAccessor = traceInfoAccessor;
            _options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string traceId = _traceInfoAccessor.TraceInfo?.TraceId;

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