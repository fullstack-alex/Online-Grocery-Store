using Microsoft.AspNetCore.Mvc;
using Model;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    [HttpGet("{email}/{password}")]
    public async Task<ActionResult<Customer?>> Get([FromRoute] string email, string password, [FromServices] CustomerService customerService)
    {
        Console.WriteLine(email + " " + password);
        Customer? customer = await customerService.GetAsync(email, password);

        if (customer.uid == null)
        {   
            return NotFound();
        }
    
        return customer;
    }
    
    [HttpGet("email/{email}")]
    public async Task<ActionResult<Customer?>> GetEmail([FromRoute] string email, [FromServices] CustomerService customerService)
    {
        Console.WriteLine(email);
        Customer? customer = await customerService.GetAsyncByEmail(email);
    
        if (customer.uid == null)
        {   
            return NotFound();
        }
    
        return customer;
    }
    
    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer, [FromServices] CustomerService customerService)
    {
        customer.uid = Guid.NewGuid().ToString();
        
        Console.WriteLine(customer.email);
        Customer? oldCustomer = await customerService.GetAsyncByEmail(customer.email);
    
        if (oldCustomer.uid != null)
        {   
            return StatusCode(409, $"User with '{customer.email}' email already exists.");
        }
        
        customer = await customerService.CreateAsync(customer);
    
        return customer;
    }
}