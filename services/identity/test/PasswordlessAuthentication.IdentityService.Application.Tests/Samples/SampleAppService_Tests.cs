using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.IdentityService.Samples;

public abstract class SampleAppService_Tests<TStartupModule> : IdentityServiceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    //private readonly ISampleAppService _sampleAppService;

    protected SampleAppService_Tests()
    {
        //_sampleAppService = GetRequiredService<ISampleAppService>();
    }

    // [Fact]
    // public async Task Method1Async()
    // {
    //
    // }
}
