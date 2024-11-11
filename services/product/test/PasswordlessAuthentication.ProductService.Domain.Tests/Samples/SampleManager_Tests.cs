using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.ProductService.Samples;

public abstract class SampleManager_Tests<TStartupModule> : ProductServiceDomainTestBase<TStartupModule>
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
