using PasswordlessAuthentication.ProductService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PasswordlessAuthentication.ProductService;

public abstract class ProductServiceController : AbpController
{
    protected ProductServiceController()
    {
        LocalizationResource = typeof(ProductServiceResource);
    }
}
