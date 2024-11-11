using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.Pro.Web;
using Volo.Abp.VirtualFileSystem;

namespace PasswordlessAuthentication.IdentityService.Web;

[DependsOn(
    typeof(AbpIdentityWebModule),
    typeof(AbpOpenIddictProWebModule),
    typeof(IdentityServiceApplicationContractsModule)
)]
public class IdentityServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<IdentityServiceWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<IdentityServiceWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<IdentityServiceWebModule>(validate: true);
        });
    }
}
