using AutoMapper;
using PasswordlessAuthentication.ProductService.Products;

namespace PasswordlessAuthentication.ProductService.Web;

public class ProductServiceWebAutoMapperProfile : Profile
{
    public ProductServiceWebAutoMapperProfile()
    {
        CreateMap<ProductDto, ProductUpdateDto>().MapExtraProperties();
    }
}
