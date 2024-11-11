using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using PasswordlessAuthentication.ProductService.Localization;
using PasswordlessAuthentication.ProductService.Permissions;
using PasswordlessAuthentication.ProductService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace PasswordlessAuthentication.ProductService.Web;

[DependsOn(
    typeof(ProductServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule)
    )]
public class ProductServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(ProductServiceResource), typeof(ProductServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ProductServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ProductServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProductServiceWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<ProductServiceWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ProductServiceWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Products/Index", ProductServicePermissions.Products.Default);
        });
        
        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<PasswordlessAuthentication.ProductService.Web.Pages.Products.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<ProductServiceResource>("NewProduct"),
                        icon: "plus",
                        name: "CreateProduct",
                        requiredPolicyName: ProductServicePermissions.Products.Create
                    );
                }
            );
        });

        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ProductServiceResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource));
        });
    }
}
