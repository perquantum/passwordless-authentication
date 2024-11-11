using PasswordlessAuthentication.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace PasswordlessAuthentication;

[DependsOn(
    typeof(AbpValidationModule)
    )]
public class PasswordlessAuthenticationSharedLocalizationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PasswordlessAuthenticationSharedLocalizationModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<PasswordlessAuthenticationResource>("en")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                ).AddVirtualJson("/Localization/PasswordlessAuthentication");

            options.DefaultResourceType = typeof(PasswordlessAuthenticationResource);
        });
    }
}
