using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Erp.WmsConnector.Shared.Infrastructure.OpenApi;

public static class RegistrationExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetRequiredOptions<GatewayMetadata>();

        services.ConfigureOptions<GatewayMetadata>(configuration);

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Name,
                Version = options.Version,
                Description = "Use the 'Authorize' button to set your JWT Bearer token for testing."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = """
                    Enter your JWT Bearer token in the format: your-jwt-token
                    
                    You can generate a development token using the /dev/generate-token endpoint.
                    """
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}