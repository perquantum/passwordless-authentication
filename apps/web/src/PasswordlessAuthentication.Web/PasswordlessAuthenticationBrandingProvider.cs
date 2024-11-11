using Microsoft.Extensions.Localization;
using PasswordlessAuthentication.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace PasswordlessAuthentication.Web;

[Dependency(ReplaceServices = true)]
public class PasswordlessAuthenticationBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<PasswordlessAuthenticationResource> _localizer;

    public PasswordlessAuthenticationBrandingProvider(IStringLocalizer<PasswordlessAuthenticationResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
