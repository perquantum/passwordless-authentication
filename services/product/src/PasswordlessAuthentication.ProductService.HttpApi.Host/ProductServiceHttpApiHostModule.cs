using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordlessAuthentication.ProductService.DbMigrations;
using PasswordlessAuthentication.ProductService.EntityFrameworkCore;
using PasswordlessAuthentication.Shared.Hosting.AspNetCore;
using PasswordlessAuthentication.Shared.Hosting.Microservices;
using Prometheus;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace PasswordlessAuthentication.ProductService;

[DependsOn(
    typeof(PasswordlessAuthenticationSharedHostingMicroservicesModule),
    typeof(ProductServiceApplicationModule),
    typeof(ProductServiceHttpApiModule),
    typeof(ProductServiceEntityFrameworkCoreModule)
)]
public class ProductServiceHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Enable if you need these
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        JwtBearerConfigurationHelper.Configure(context, "ProductService");
        SwaggerConfigurationHelper.ConfigureWithOidc(
            context: context,
            authority: configuration["AuthServer:Authority"]!,
            scopes: new[] { "ProductService" },
            flows: new[] { "authorization_code" },
            discoveryEndpoint: configuration["AuthServer:MetadataAddress"],
            apiTitle: "Product Service API"
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
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

        context.Services.TransformAbpClaims();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCorrelationId();
        app.UseAbpRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        app.UseMultiTenancy();
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        });
        app.UseAbpSerilogEnrichers();
        app.UseAuditing();
        app.UseConfiguredEndpoints(endpoints => endpoints.MapMetrics());
    }

    public async override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();

        if (!env.IsDevelopment())
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<ProductServiceDatabaseMigrationChecker>()
                    .CheckAndApplyDatabaseMigrationsAsync();
            }
        }
    }
}
