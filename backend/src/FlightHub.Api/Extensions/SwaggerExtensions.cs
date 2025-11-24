using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FlightHub.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FlightHub API",
                Version = "v1",
                Description = "Flight information API for the coding challenge."
            });

            // include XML comments for Api, Application, Domain
            var basePath = AppContext.BaseDirectory;

            var xmlFiles = Directory
                .GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var xmlPath in xmlFiles)
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
