using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.TraceInfoSection;

namespace WebApplication.Middleware
{
    public class LogScopeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public LogScopeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context, ITraceInfoAccessor traceInfoAccessor)
        {
            var logger = _loggerFactory.CreateLogger<LogScopeMiddleware>();
            using (logger.BeginScope($"{traceInfoAccessor.TraceInfo?.TraceId}--{traceInfoAccessor.TraceInfo?.MachineName}"))
            {
                await _next.Invoke(context);
            }
        }
    }
}