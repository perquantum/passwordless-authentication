using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.ProductService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(ProductServiceDomainModule),
    typeof(ProductServiceTestBaseModule)
    )]
public class ProductServiceDomainTestModule : AbpModule
{

}
