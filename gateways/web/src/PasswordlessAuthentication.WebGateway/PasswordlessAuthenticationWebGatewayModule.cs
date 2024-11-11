using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordlessAuthentication.Shared.Hosting.AspNetCore;
using PasswordlessAuthentication.Shared.Hosting.Gateways;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp;
using Volo.Abp.Modularity;
using Yarp.ReverseProxy.Configuration;

namespace PasswordlessAuthentication.WebGateway;

[DependsOn(
    typeof(PasswordlessAuthenticationSharedHostingGatewaysModule)
)]
public class PasswordlessAuthenticationWebGatewayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Enable if you need hosting environment
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        SwaggerConfigurationHelper
            .ConfigureWithOidc(
                context: context,
                authority: configuration["AuthServer:Authority"]!,
                scopes: new[] {
                    /* Requested scopes for authorization code request and descriptions for swagger UI only */
                    "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
                },
                apiTitle: "Web Gateway API",
                discoveryEndpoint: configuration["AuthServer:MetadataAddress"]
            );

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();
        var proxyConfig = app.ApplicationServices.GetRequiredService<IProxyConfigProvider>().GetConfig();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options => { ConfigureSwaggerUI(proxyConfig, options, configuration); });
        app.UseRewriter(CreateSwaggerRewriteOptions());
        app.UseAbpSerilogEnrichers();
        app.UseEndpoints(endpoints => endpoints.MapReverseProxy());
    }
    
    private static void ConfigureSwaggerUI(
        IProxyConfig proxyConfig,
        SwaggerUIOptions options,
        IConfiguration configuration)
    {
        foreach (var cluster in proxyConfig.Clusters)
        {
            options.SwaggerEndpoint($"/swagger-json/{cluster.ClusterId}/swagger/v1/swagger.json",
                $"{cluster.ClusterId} API");
        }

        options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        options.OAuthScopes(
            "AdministrationService",
            "AccountService",
            "IdentityService",
            "SaasService",
            "ProductService"
        );
    }

    private static RewriteOptions CreateSwaggerRewriteOptions()
    {
        var rewriteOptions = new RewriteOptions();
        rewriteOptions.AddRedirect("^(|\\|\\s+)$", "/swagger"); // Regex for "/" and "" (whitespace)
        return rewriteOptions;
    }
}