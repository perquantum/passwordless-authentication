using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.IdentityService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class IdentityServiceApplicationTestBase<TStartupModule> : IdentityServiceTestBase<TStartupModule>
 where TStartupModule : IAbpModule
{

}
