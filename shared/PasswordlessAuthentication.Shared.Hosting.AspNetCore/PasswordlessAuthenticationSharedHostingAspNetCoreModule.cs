using Microsoft.Extensions.Hosting;
using Prometheus;
using Volo.Abp;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace PasswordlessAuthentication.Shared.Hosting.AspNetCore;

[DependsOn(
    typeof(PasswordlessAuthenticationSharedHostingModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class PasswordlessAuthenticationSharedHostingAspNetCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseMetricServer();
        }
    }
}
