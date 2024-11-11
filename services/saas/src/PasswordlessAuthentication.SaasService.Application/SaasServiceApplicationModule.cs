using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace PasswordlessAuthentication.SaasService.Application;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(SaasServiceDomainModule),
    typeof(SaasTenantApplicationModule),
    typeof(SaasHostApplicationModule)
)]
public class SaasServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<SaasServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SaasServiceApplicationModule>(validate: true);
        });
    }
}
