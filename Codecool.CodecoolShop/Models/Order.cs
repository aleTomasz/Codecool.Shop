using System.Collections.Generic;

namespace Codecool.CodecoolShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<ListItems> Items { get; set; } = new List<ListItems>();
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }

        public decimal TotalPrice => CalculateTotalPrice();
        private decimal CalculateTotalPrice()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.TotalPrice;
            }
            return total;
        }
    }
    public class AddressModel
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
    }
}
