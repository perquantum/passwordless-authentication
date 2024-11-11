using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.ProductService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class ProductServiceDomainTestBase<TStartupModule> : ProductServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
