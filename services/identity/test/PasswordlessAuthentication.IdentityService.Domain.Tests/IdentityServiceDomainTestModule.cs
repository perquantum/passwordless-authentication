﻿using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.IdentityService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(IdentityServiceDomainModule),
    typeof(IdentityServiceTestBaseModule)
)]
public class IdentityServiceDomainTestModule : AbpModule
{

}
