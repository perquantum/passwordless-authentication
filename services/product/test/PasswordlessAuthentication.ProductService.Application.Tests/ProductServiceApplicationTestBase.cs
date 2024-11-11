using PasswordlessAuthentication.ProductService;
using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.AdministrationService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class ProductServiceApplicationTestBase<TStartupModule> : ProductServiceTestBase<TStartupModule>
 where TStartupModule : IAbpModule
{

}
