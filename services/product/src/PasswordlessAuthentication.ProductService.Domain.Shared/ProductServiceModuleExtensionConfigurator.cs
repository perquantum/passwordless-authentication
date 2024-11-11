using Volo.Abp.Threading;

namespace PasswordlessAuthentication.ProductService;

public static class ProductServiceModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /* You can configure extra properties for the
         * entities defined in the modules used by your application.
         *
         * This class can be used to define these extra properties
         * with a high level, easy to use API.
         *
         * Example: Add a new property to the user entity of the identity module
           ObjectExtensionManager.Instance.Modules()
            .ConfigureProductService(productService =>
            {
                productService.ConfigureProduct(product =>
                {
                    product.AddOrUpdateProperty<string>( //property type: string
                        "Description", //property name
                        property =>
                        {
                            //validation rules
                            property.Attributes.Add(new RequiredAttribute());
                            property.Attributes.Add(new StringLengthAttribute(64) {MinimumLength = 4});
                            //...other configurations for this property
                        }
                    );
                });
            });
         * 
         * 
         * See the documentation for more:
         * https://abp.io/docs/latest/framework/architecture/modularity/extending/module-entity-extensions
         */
    }
}
