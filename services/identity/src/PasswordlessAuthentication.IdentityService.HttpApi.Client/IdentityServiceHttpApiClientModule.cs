using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.IdentityService;

[DependsOn(
    typeof(IdentityServiceApplicationContractsModule),
    typeof(AbpOpenIddictProHttpApiClientModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpAccountAdminHttpApiClientModule)
    )]
public class IdentityServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(IdentityServiceApplicationContractsModule).Assembly,
            IdentityServiceRemoteServiceConsts.RemoteServiceName
        );
    }
}
