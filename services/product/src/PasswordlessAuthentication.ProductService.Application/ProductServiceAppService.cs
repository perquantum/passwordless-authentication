using PasswordlessAuthentication.ProductService.Localization;
using Volo.Abp.Application.Services;

namespace PasswordlessAuthentication.ProductService;

public abstract class ProductServiceAppService : ApplicationService
{
    protected ProductServiceAppService()
    {
        LocalizationResource = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceApplicationModule);
    }
}
