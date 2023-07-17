using Microsoft.AspNetCore.Mvc;
using Model;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    [HttpGet("{customerUid}")]
    public async Task<ActionResult<List<ShoppingCartItem>>> Get([FromRoute] string customerUid, [FromServices] CartService cartService)
    {
        Console.WriteLine(customerUid);
        List<ShoppingCartItem> shoppingCart = await cartService.GetShoppingCart(customerUid);
    
        return shoppingCart;
    }
    
    [HttpPost]
    public async Task<ActionResult<ShoppingCartItem>> Post(ShoppingCartItem cartItem, [FromServices] CartService cartService)
    {
        Console.WriteLine(cartItem.productUid);
        
        ShoppingCartItem shoppingCartItem = await cartService.CreateAsync(cartItem);

        return shoppingCartItem;
    }
}