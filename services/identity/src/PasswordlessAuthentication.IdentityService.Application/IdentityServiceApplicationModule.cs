using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace PasswordlessAuthentication.IdentityService;

[DependsOn(
    typeof(AbpIdentityApplicationModule),
    typeof(AbpOpenIddictProApplicationModule),
    typeof(IdentityServiceDomainModule),
    typeof(AbpAccountAdminApplicationModule),
    typeof(IdentityServiceApplicationContractsModule)
)]
public class IdentityServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<IdentityServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<IdentityServiceApplicationModule>(validate: true);
        });
    }
}
