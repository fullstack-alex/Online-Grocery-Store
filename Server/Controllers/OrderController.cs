using Microsoft.AspNetCore.Mvc;
using Model;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    [HttpGet("{customerUid}")]
    public async Task<ActionResult<List<Order>>> Get([FromRoute] string customerUid, [FromServices] OrderService orderService)
    {
        Console.WriteLine("customerUid " + customerUid);
        List<Order> orders = await orderService.GetOrdersByCustomerAsync(customerUid);
    
        return orders;
    }
    
    [HttpPost("{customerUid}")]
    public async Task<ActionResult<Order>> Post([FromRoute] string customerUid, [FromServices] OrderService orderService, [FromServices] CartService cartService,  [FromServices] ProductService productService)
    {
        Order order = new Order();
        order.customerUid = customerUid;
        order.uid = Guid.NewGuid().ToString();
        order.date = DateTime.Now;
        decimal totalPrice = 0;
        List<ShoppingCartItem> cartItems = await cartService.GetShoppingCart(customerUid);

        foreach (ShoppingCartItem cartItem in cartItems)
        {
            Product product = await productService.GetAsyncByUid(cartItem.productUid);

            totalPrice += product.price * cartItem.quantity;
            
            await cartService.DeleteAsync(cartItem);
        }

        order.totalPrice = totalPrice;
        
        Console.WriteLine(order.customerUid);
        
        order = await orderService.CreateAsync(order);
    
        return order;
    }
}