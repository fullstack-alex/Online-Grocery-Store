namespace Model;

public class Order
{
    public string uid { get; set; }
    public string customerUid { get; set; }
    public decimal totalPrice { get; set; }
    public DateTime date { get; set; }
}
