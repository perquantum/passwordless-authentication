using Microsoft.Extensions.DependencyInjection;
using PasswordlessAuthentication.ProductService.Products;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace PasswordlessAuthentication.ProductService.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreMySQLModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(ProductServiceDomainModule)
)]
public class ProductServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ProductServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ProductServiceDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<Product, EfCoreProductRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<ProductServiceDbContext>(c =>
            {
                c.UseMySQL(b =>
                {
                    b.MigrationsHistoryTable("__ProductService_Migrations");
                });
            });
        });
    }
}
