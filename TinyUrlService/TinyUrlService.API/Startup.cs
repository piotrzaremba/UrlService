using System.Reflection;
using TinyUrlService.API.Infrastructure;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using FluentValidation;
using Microsoft.OpenApi.Models;
using MediatR;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Handlers;
using TinyUrlService.Domain.Services;
using TinyUrlService.Domain.Services.Queries;
using TinyUrlService.Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace TinyUrlService.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TinyURL API", Version = "v1" });
            });

            services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService("TinyUrlService"))
                    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation().AddConsoleExporter())
                    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation());

            // Required for sessions.
            services.AddDistributedMemoryCache();
            /*
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            */

            services.AddCors(options =>
            {
                options.AddPolicy("ReactPolicy",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000") // Replace with your React application's URL
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".TinyUrlService.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // No SSL certs required.
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None; // We are not sending any sensative data.
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // I don't want to create a self signed cert, otherwise changes this to Always.
            });


            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddSingleton<IUrlService, UrlService>();

            services.AddScoped<IValidator<CreateShortUrlCommand>, CreateShortUrlValidator>();
            services.AddScoped<IValidator<DeleteShortUrlCommand>, DeleteShortUrlValidator>();
            services.AddScoped<IValidator<GetLongUrlQuery>, GetLongUrlValidator>();
            services.AddScoped<IValidator<GetStatisticsQuery>, GetStatisticsValidator>();


            services.AddScoped(typeof(IRequestHandler<CreateShortUrlCommand, string>), typeof(CreateShortUrlHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteShortUrlCommand, bool>), typeof(DeleteShortUrlHandler));
            services.AddScoped(typeof(IRequestHandler<GetLongUrlQuery, UrlMapping>), typeof(GetLongUrlHandler));
            services.AddScoped(typeof(IRequestHandler<GetStatisticsQuery, Dictionary<string, UrlMapping>>), typeof(GetStatisticsHandler));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionMiddleware>();
            }

            app.UseRouting();

            app.UseCors("ReactPolicy");

            app.UseSession();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TinyURL API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
