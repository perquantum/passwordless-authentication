using Volo.Abp.Modularity;
using Volo.Saas;

namespace PasswordlessAuthentication.SaasService;

[DependsOn(
    typeof(SaasServiceDomainSharedModule),
    typeof(SaasDomainModule)
)]
public class SaasServiceDomainModule : AbpModule
{
}
