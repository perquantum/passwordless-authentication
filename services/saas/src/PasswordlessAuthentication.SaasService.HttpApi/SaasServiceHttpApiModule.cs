using Volo.Abp.Modularity;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace PasswordlessAuthentication.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(SaasHostHttpApiModule),
    typeof(SaasTenantHttpApiModule)
)]
public class SaasServiceHttpApiModule : AbpModule
{
}
