using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.SaasService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class SaasServiceApplicationTestBase<TStartupModule> : SaasServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
