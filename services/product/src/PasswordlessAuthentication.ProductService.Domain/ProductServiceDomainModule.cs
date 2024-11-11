using PasswordlessAuthentication.ProductService.Products.ObjectExtending;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;

namespace PasswordlessAuthentication.ProductService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ProductServiceDomainSharedModule)
)]
public class ProductServiceDomainModule : AbpModule
{
    private readonly static OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                ProductServiceExtensionConsts.ModuleName,
                ProductServiceExtensionConsts.EntityNames.Product,
                typeof(Products.Product)
            );
        });
    }
}
