namespace Codecool.CodecoolShop.Models
{
    public class ListItems
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Product.DefaultPrice * Quantity;
    }
}
