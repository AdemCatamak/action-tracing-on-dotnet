using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.HttpInterceptors;
using WebApplication.Middleware;
using WebApplication.Proxies;
using WebApplication.TraceInfoSection;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTraceInfoAccessor();
            
            services.AddTransient<HttpClientTraceIdInterceptor>();
            services.AddHttpClient<ICityClient, CityHttpClient>(httpClient =>
                                                                {
                                                                    httpClient.BaseAddress = new Uri("http://localhost:5000");

                                                                    httpClient.DefaultRequestHeaders.Accept
                                                                              .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                                })
                    .AddHttpMessageHandler<HttpClientTraceIdInterceptor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<TraceIdMiddleware>();
//            var traceIdMiddlewareConfiguration = new TraceIdMiddlewareConfiguration {MachineNameIncludeInResponseHeader = false};
//            app.UseMiddleware<TraceIdMiddleware>(Options.Create(traceIdMiddlewareConfiguration));
            app.UseMiddleware<LogScopeMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

//            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}