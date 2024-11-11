using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.AdministrationService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AdministrationServiceDomainModule),
    typeof(AdministrationServiceTestBaseModule)
)]
public class AdministrationServiceDomainTestModule : AbpModule
{

}
