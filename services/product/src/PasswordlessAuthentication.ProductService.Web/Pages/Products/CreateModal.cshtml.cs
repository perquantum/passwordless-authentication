using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordlessAuthentication.ProductService.Products;

namespace PasswordlessAuthentication.ProductService.Web.Pages.Products;

public class CreateModalModel : ProductServicePageModel
{
    [BindProperty]
    public ProductCreateDto Product { get; set; } = default!;

    private readonly IProductAppService _productAppService;

    public CreateModalModel(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    public void OnGet()
    {
        Product = new ProductCreateDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _productAppService.CreateAsync(Product);
        return NoContent();
    }
}
