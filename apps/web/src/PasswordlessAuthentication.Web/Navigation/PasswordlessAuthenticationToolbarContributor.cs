using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PasswordlessAuthentication.Web.Components.Toolbar.LoginLink;
using PasswordlessAuthentication.Web.Components.Toolbar.Impersonation;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Users;

namespace PasswordlessAuthentication.Web.Navigation;

public class PasswordlessAuthenticationToolbarContributor : IToolbarContributor
{
    public virtual Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
    {
        if (context.Toolbar.Name != StandardToolbars.Main)
        {
            return Task.CompletedTask;
        }

        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        if (!currentUser.IsAuthenticated)
        {
            context.Toolbar.Items.Add(new ToolbarItem(typeof(LoginLinkViewComponent)));
        }

        if (currentUser.FindImpersonatorUserId() != null)
        {
            context.Toolbar.Items.Add(new ToolbarItem(typeof(ImpersonationViewComponent), order: -1));
        }

        return Task.CompletedTask;
    }
}
