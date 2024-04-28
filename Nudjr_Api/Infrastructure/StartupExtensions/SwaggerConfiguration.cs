using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Nudjr_Api.Infrastructure.StartupExtensions
{
    public static class SwaggerConfiguration
    {

        public static IServiceCollection RegisterAndConfigureSwaggerAuthorizationOptions(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                OpenApiSecurityRequirement security = new OpenApiSecurityRequirement
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
                };

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                x.AddSecurityRequirement(security);
                x.IncludeXmlComments(GetXmlCommentsPath(), includeControllerXmlComments: true);
            });
            return services;
        }

        private static string GetXmlCommentsPath()
        {
            string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string filePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
            return filePath;
        }
    }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            OpenApiInfo info = new OpenApiInfo();

            if (description.GroupName == "v1")
            {
                info = new OpenApiInfo
                {
                    Title = "Nudjr API",
                    Version = description.ApiVersion.ToString(),
                    Description = "This is the Version 1 Of Nudjr that includes Phase 1 features."
                };
            }

            if (description.GroupName == "v2")
            {
                info = new OpenApiInfo
                {
                    Title = "Nudjr API",
                    Version = description.ApiVersion.ToString(),
                    Description = "This is the Version 2 Of Nudjr that includes Phase 2 features."
                };
            }

            return info;
        }
    }
}
