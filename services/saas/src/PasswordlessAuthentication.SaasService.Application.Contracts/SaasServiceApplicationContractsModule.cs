using Volo.Abp.Modularity;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace PasswordlessAuthentication.SaasService;

[DependsOn(
    typeof(SaasTenantApplicationContractsModule),
    typeof(SaasHostApplicationContractsModule),
    typeof(SaasServiceDomainSharedModule)
)]
public class SaasServiceApplicationContractsModule : AbpModule
{
}
