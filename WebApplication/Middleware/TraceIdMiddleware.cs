using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace WebApplication.Middleware
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<TraceIdMiddlewareConfiguration> _traceIdMiddlewareConfigurationOptions;

        public TraceIdMiddleware(RequestDelegate next, IOptions<TraceIdMiddlewareConfiguration> options = null)
        {
            _next = next;
            _traceIdMiddlewareConfigurationOptions = options;
        }

        public async Task Invoke(HttpContext context)
        {
            TraceIdMiddlewareConfiguration traceIdMiddlewareConfiguration = _traceIdMiddlewareConfigurationOptions?.Value ?? new TraceIdMiddlewareConfiguration();

            if (!context.Request.Headers.TryGetValue(traceIdMiddlewareConfiguration.TraceIdHeader, out StringValues traceId))
            {
                DateTime requestTime = DateTime.UtcNow;
                traceId = $"{requestTime:yyyy-MM-dd-HH-mm-ss}--{context.Connection.Id}";
            }

            context.TraceIdentifier = traceId;
            
            context.Response.OnStarting(() =>
                                        {
                                            if (traceIdMiddlewareConfiguration.TraceIdIncludeInResponseHeader)
                                            {
                                                context.Response.Headers.Add(traceIdMiddlewareConfiguration.TraceIdHeader, context.TraceIdentifier);
                                            }

                                            if (traceIdMiddlewareConfiguration.MachineNameIncludeInResponseHeader)
                                            {
                                                context.Response.Headers.Add(traceIdMiddlewareConfiguration.MachineNameHeader, Environment.MachineName);
                                            }

                                            return Task.CompletedTask;
                                        });

            await _next.Invoke(context);
        }
    }

    public class TraceIdMiddlewareConfiguration
    {
        public const string DefaultTraceIdHeader = "x-trace-id";
        public const string DefaultMachineNameHeader = "x-machine-name";

        public string MachineNameHeader { get; set; } = DefaultMachineNameHeader;
        public string TraceIdHeader { get; set; } = DefaultTraceIdHeader;

        public bool TraceIdIncludeInResponseHeader { get; set; } = true;
        public bool MachineNameIncludeInResponseHeader { get; set; } = true;
    }
}