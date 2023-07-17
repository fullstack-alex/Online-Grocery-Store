using Model;
using MySqlConnector;

namespace Server.Services;

public class CartService
{
    private MySqlConnection connection;
    
    public CartService()
    {
        try
        {
            connection = new MySqlConnection("Server=localhost;User ID=root;Password=pass;Database=ab;Allow User Variables=true");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<ShoppingCartItem>> GetShoppingCart(string customerUid)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM shoppingcartitems WHERE customerUid=@customerUid;", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", customerUid));
        using var reader = await command.ExecuteReaderAsync();

        List<ShoppingCartItem> shoppingCart = new List<ShoppingCartItem>();

        while (await reader.ReadAsync())
        {
            ShoppingCartItem shoppingCartItem = new ShoppingCartItem();
            try
            {
                shoppingCartItem.customerUid = reader.GetValue(0).ToString();
                shoppingCartItem.productUid = reader.GetValue(1).ToString();
                shoppingCartItem.quantity = (int) reader.GetValue(2);

                shoppingCart.Add(shoppingCartItem);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        return shoppingCart;
    }

    public async Task<ShoppingCartItem?> CreateAsync(ShoppingCartItem cartItem)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("SELECT * FROM shoppingcartitems WHERE customerUid=@customerUid and productUid=@productUid;", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", cartItem.customerUid));
        command.Parameters.Add(new MySqlParameter("productUid", cartItem.productUid));
        using var reader = await command.ExecuteReaderAsync();
        
        ShoppingCartItem? existingCartItem = new ShoppingCartItem();
        while (await reader.ReadAsync())
        {
            try
            {
                existingCartItem.customerUid = reader.GetValue(0).ToString();
                existingCartItem.productUid = reader.GetValue(1).ToString();
                existingCartItem.quantity = (int) reader.GetValue(2);
            }
            catch (Exception e)
            {
                existingCartItem = null;
                Console.WriteLine(e);
            }
        }


        int quantityToSet = existingCartItem.quantity + cartItem.quantity < 0
            ? 0
            : existingCartItem.quantity + cartItem.quantity;
        
        if (existingCartItem == null || existingCartItem.customerUid == null || existingCartItem.productUid == null)
        {
            if (cartItem.quantity < 0) cartItem.quantity = 0;
            await InsertAsync(cartItem);
        }
        else
        {
            await UpdateAsync(existingCartItem, quantityToSet);
            cartItem.quantity = quantityToSet;
        }

        if (cartItem.quantity <= 0)
        {
            DeleteAsync(cartItem);
        }
    
        return cartItem;
    }
    
    

    private async Task UpdateAsync(ShoppingCartItem existingCartItem, int quantityToSet)
    {
        try
        {
            await connection.CloseAsync();
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("UPDATE shoppingcartitems SET quantity = @quantityToSet WHERE customerUid=@customerUid and productUid=@productUid", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", existingCartItem.customerUid));
        command.Parameters.Add(new MySqlParameter("productUid", existingCartItem.productUid));
        command.Parameters.Add(new MySqlParameter("quantityToSet", quantityToSet));
        using var reader = await command.ExecuteReaderAsync();
    }

    private async Task InsertAsync(ShoppingCartItem cartItem)
    {
        try
        {
            await connection.CloseAsync();
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("INSERT INTO shoppingcartitems(customerUid,productUid,quantity) VALUES(@customerUid, @productUid, @quantity)", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", cartItem.customerUid));
        command.Parameters.Add(new MySqlParameter("productUid", cartItem.productUid));
        command.Parameters.Add(new MySqlParameter("quantity", cartItem.quantity));
        using var reader = await command.ExecuteReaderAsync();
    }

    public async Task DeleteAsync(ShoppingCartItem cartItem)
    {
        try
        {
            await connection.CloseAsync();
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("DELETE FROM shoppingcartitems WHERE customerUid=@customerUid and productUid=@productUid", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", cartItem.customerUid));
        command.Parameters.Add(new MySqlParameter("productUid", cartItem.productUid));
        using var reader = await command.ExecuteReaderAsync();
    }
}