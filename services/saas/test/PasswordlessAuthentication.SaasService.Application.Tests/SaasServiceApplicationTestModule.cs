using PasswordlessAuthentication.SaasService.Application;
using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationModule),
    typeof(SaasServiceDomainTestModule)
    )]
public class SaasServiceApplicationTestModule : AbpModule
{

}
