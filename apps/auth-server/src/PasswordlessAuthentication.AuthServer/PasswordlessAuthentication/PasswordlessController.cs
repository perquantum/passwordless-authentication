using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity;

namespace PasswordlessAuthentication.AuthServer.PasswordlessAuthentication;

[RemoteService(Name = AccountProPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AccountProPublicRemoteServiceConsts.ModuleName)]
[Route("api/account")]
public class PasswordlessController : AbpController
{
    protected IdentityUserManager UserManager { get; }

    protected IdentityRoleManager RoleManager { get; }

    protected AbpSignInManager SignInManager { get; }

    public PasswordlessController(
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        AbpSignInManager signInManager)
    {
        UserManager = userManager;
        RoleManager = roleManager;
        SignInManager = signInManager;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("passwordless-authentication")]
    public async Task<Result> VerifyOTPAndSignIn(Input input)
    {

        var user = await UserManager.FindByNameAsync(input.Username);
        var token = await UserManager.GenerateUserTokenAsync(user, "PasswordlessLoginProvider", "passwordless-auth");

        await UserManager.UpdateSecurityStampAsync(user);
        await SignInManager.SignInAsync(user, isPersistent: true);
        return
            new Result()
            {
                Token = token,
                UserId = user.Id
            };
    }
}