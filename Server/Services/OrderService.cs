using Model;
using MySqlConnector;

namespace Server.Services;

public class OrderService
{
    private MySqlConnection connection;
    
    public OrderService()
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
    
    public async Task<List<Order>> GetOrdersByCustomerAsync(string customerUid)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM orders WHERE customerUid=@customerUid;", connection);
        command.Parameters.Add(new MySqlParameter("customerUid", customerUid));
        using var reader = await command.ExecuteReaderAsync();

        List<Order> orders = new List<Order>();

        while (await reader.ReadAsync())
        {
            Order order = new Order();
            try
            {
                order.uid = reader.GetValue(0).ToString();
                order.customerUid = reader.GetValue(1).ToString();
                order.totalPrice = (decimal) reader.GetValue(2);
                order.date = (DateTime) reader.GetValue(3);

                orders.Add(order);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        return orders;
    }
    
    public async Task<Order?> CreateAsync(Order order)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("INSERT INTO orders(uid,customerUid,totalPrice,date) VALUES(@uid, @customerUid, @totalPrice, @date)", connection);
        command.Parameters.Add(new MySqlParameter("uid", order.uid));
        command.Parameters.Add(new MySqlParameter("customerUid", order.customerUid));
        command.Parameters.Add(new MySqlParameter("totalPrice", order.totalPrice));
        command.Parameters.Add(new MySqlParameter("date", order.date));
        using var reader = await command.ExecuteReaderAsync();
    
        return order;
    }
}