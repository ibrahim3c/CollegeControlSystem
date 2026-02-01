
using ByteStore.Api.Extenstions;
using CollegeControlSystem.Application;
using CollegeControlSystem.Infrastructure;
using CollegeControlSystem.Presentation.Extenstions;
using CollegeControlSystem.Presentation.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CollegeControlSystem.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDependencyInjectionService(builder.Configuration);
            builder.Services.AddRateLimiting();
            builder.Services.AddHealthCheck(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseRateLimiter();
            app.UseHttpsRedirection();

            // custom middlewares
            app.UseCustomCors();
            app.UseCustomExceptionHandler();

            app.UseAuthorization();


            app.MapControllers();

            // it cause problem of more than dbContext was found
            //  Map Health Checks JSON Endpoint => normal health check just api return json response
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                //This formats the response in a special JSON format that the HealthCheckUI dashboard understands.
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });


            // Map HealthCheck UI Dashboard
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui"; // Dashboard path
                options.ApiPath = "/health-ui-api"; // API used by the UI
            });


            app.Run();
        }
    }
}
