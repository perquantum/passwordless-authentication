using PasswordlessAuthentication.AdministrationService;
using PasswordlessAuthentication.AdministrationService.EntityFrameworkCore;
using PasswordlessAuthentication.IdentityService;
using PasswordlessAuthentication.IdentityService.EntityFrameworkCore;
using PasswordlessAuthentication.ProductService;
using PasswordlessAuthentication.ProductService.EntityFrameworkCore;
using PasswordlessAuthentication.SaasService;
using PasswordlessAuthentication.SaasService.EntityFrameworkCore;
using PasswordlessAuthentication.Shared.Hosting;
using Volo.Abp.Modularity;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;

namespace PasswordlessAuthentication.DbMigrator;

[DependsOn(
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(PasswordlessAuthenticationSharedHostingModule),
    typeof(IdentityServiceEntityFrameworkCoreModule),
    typeof(IdentityServiceApplicationContractsModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(SaasServiceApplicationContractsModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(ProductServiceApplicationContractsModule),
    typeof(ProductServiceEntityFrameworkCoreModule)
)]
public class PasswordlessAuthenticationDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "PasswordlessAuthentication:"; });
    }
}
