using Model;
using MySqlConnector;

namespace Server.Services;

public class CustomerService
{
    private MySqlConnection connection;
    
    public CustomerService()
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
    
    public async Task<Customer?> GetAsync(string email, string password)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM customers WHERE email=@email and password=@password;", connection);
        command.Parameters.Add(new MySqlParameter("email", email));
        command.Parameters.Add(new MySqlParameter("password", password));
        using var reader = await command.ExecuteReaderAsync();
        
        Customer? value = new Customer();
        while (await reader.ReadAsync())
        {
            try
            {
                value.uid = reader.GetValue(0).ToString();
                value.name = reader.GetValue(1).ToString();
                value.email = reader.GetValue(2).ToString();
                value.password = reader.GetValue(3).ToString();
            }
            catch (Exception e)
            {
                value = null;
                Console.WriteLine(e);
            }
        }
        
        return value;
    }
    
    public async Task<Customer?> GetAsyncByEmail(string email)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM customers WHERE uid=@email;", connection);
        command.Parameters.Add(new MySqlParameter("email", email));
        using var reader = await command.ExecuteReaderAsync();
        
        Customer? value = new Customer();
        while (await reader.ReadAsync())
        {
            try
            {
                value.uid = reader.GetValue(0).ToString();
                value.name = reader.GetValue(1).ToString();
                value.email = reader.GetValue(2).ToString();
                value.password = reader.GetValue(3).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        return value;
    }
    
    public async Task<Customer?> CreateAsync(Customer customer)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("INSERT INTO customers(uid,name,email,password) VALUES(@uid, @name, @email, @password)", connection);
        command.Parameters.Add(new MySqlParameter("uid", customer.uid));
        command.Parameters.Add(new MySqlParameter("name", customer.name));
        command.Parameters.Add(new MySqlParameter("email", customer.email));
        command.Parameters.Add(new MySqlParameter("password", customer.password));
        using var reader = await command.ExecuteReaderAsync();
    
        return customer;
    }
}