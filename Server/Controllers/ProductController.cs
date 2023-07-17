using Microsoft.AspNetCore.Mvc;
using Model;
using Server.Services;

namespace Server.Controllers;


[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("{category}")]
    public async Task<ActionResult<List<Product>>> Get([FromRoute] string category, [FromServices] ProductService productService)
    {
        Console.WriteLine(category);
        List<Product> products = await productService.GetAsync(category);
    
        return products;
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product, [FromServices] ProductService productService)
    {
        product.uid = Guid.NewGuid().ToString();
        
        if(string.IsNullOrEmpty(product.name))
        {
            return StatusCode(400, $"The name field is required.");
        }
        else if(string.IsNullOrEmpty(product.category))
        {
            return StatusCode(400, $"The category field is required.");
        }
        else if (product.price <= 0)
        {
            return StatusCode(400, $"The price field should not be <= 0.");
        }

        Console.WriteLine(product.name);
        
        product = await productService.CreateAsync(product);
    
        return product;
    }
}