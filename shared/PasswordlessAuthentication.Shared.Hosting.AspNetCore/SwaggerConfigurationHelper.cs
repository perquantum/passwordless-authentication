using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.Shared.Hosting.AspNetCore;

public static class SwaggerConfigurationHelper
{
    public static void Configure(
        ServiceConfigurationContext context,
        string apiTitle
    )
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = apiTitle, Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }
    
    public static void ConfigureWithOidc(
        ServiceConfigurationContext context,
        string authority,
        string[] scopes,
        string apiTitle,
        string apiVersion = "v1",
        string apiName = "v1",
        string[]? flows = null,
        string? discoveryEndpoint = null
    )
    {
        context.Services.AddAbpSwaggerGenWithOidc(
            authority: authority,
            scopes: scopes,
            flows: flows,
            discoveryEndpoint: discoveryEndpoint,
            options =>
            {
                options.SwaggerDoc(apiName, new OpenApiInfo { Title = apiTitle, Version = apiVersion });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }
}
