using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.SaasService.Samples;

public abstract class SampleManager_Tests<TStartupModule> : SaasServiceDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    //private readonly SampleManager _sampleManager;

    protected SampleManager_Tests()
    {
        //_sampleManager = GetRequiredService<SampleManager>();
    }

    // [Fact]
    // public async Task Method1Async()
    // {
    //
    // }
}
