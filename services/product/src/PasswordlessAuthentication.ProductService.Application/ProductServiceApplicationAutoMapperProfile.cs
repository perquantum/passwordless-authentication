using AutoMapper;
using PasswordlessAuthentication.ProductService.Products;

namespace PasswordlessAuthentication.ProductService;

public class ProductServiceApplicationAutoMapperProfile : Profile
{
    public ProductServiceApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().MapExtraProperties();
    }
}
