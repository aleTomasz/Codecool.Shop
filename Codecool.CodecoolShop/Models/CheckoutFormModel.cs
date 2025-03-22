using System.ComponentModel.DataAnnotations;

namespace Codecool.CodecoolShop.Models
{
    public class CheckoutFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Phone { get; set; }

        public AdressModel BillingAddress { get; set; }
        public AdressModel ShippingAddress { get; set; }
    }
    public class AdressModel
    {
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }

        [RegularExpression(@"^\d{2}(-\d{3})?$", ErrorMessage = "Invalid Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
