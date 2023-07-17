using Model;
using MySqlConnector;

namespace Server.Services;

public class ProductService
{
    private MySqlConnection connection;
    
    public ProductService()
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
    
    public async Task<Product> GetAsyncByUid(string productUid)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM products WHERE uid=@uid;", connection);
        command.Parameters.Add(new MySqlParameter("uid", productUid));
        using var reader = await command.ExecuteReaderAsync();
        
        Product? value = new Product();
        while (await reader.ReadAsync())
        {
            try
            {
                value.uid = reader.GetValue(0).ToString();
                value.name = reader.GetValue(1).ToString();
                value.category = reader.GetValue(2).ToString();
                value.price = (decimal) reader.GetValue(3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        return value;
    }
    
    public async Task<List<Product>> GetAsync(string category)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        using var command = new MySqlCommand("SELECT * FROM products WHERE category=@category;", connection);
        command.Parameters.Add(new MySqlParameter("category", category));
        using var reader = await command.ExecuteReaderAsync();
        
        List<Product> productResults = new List<Product>();
        
        while (await reader.ReadAsync())
        {
           Product product = new Product();
           try
           {
               product.uid = reader.GetValue(0).ToString();
               product.name = reader.GetValue(1).ToString();
               product.category = reader.GetValue(2).ToString();
               product.price = (decimal) reader.GetValue(3);

               productResults.Add(product);
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
           }
        }
        
        return productResults;
    }
    
    public async Task<Product?> CreateAsync(Product product)
    {
        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        using var command = new MySqlCommand("INSERT INTO products(uid,name,category,price) VALUES(@uid, @name, @category, @price)", connection);
        command.Parameters.Add(new MySqlParameter("uid", product.uid));
        command.Parameters.Add(new MySqlParameter("name", product.name));
        command.Parameters.Add(new MySqlParameter("category", product.category));
        command.Parameters.Add(new MySqlParameter("price", product.price));
        using var reader = await command.ExecuteReaderAsync();
    
        return product;
    }
}