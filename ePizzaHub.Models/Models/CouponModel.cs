using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class CouponModel : BaseModel
    {
        [Required]
        public required string f_code { get; set; } // e.g., PIZZA50
        public decimal f_discount_percentage { get; set; }
        public decimal f_max_discount_amount { get; set; }
        public DateTime f_expiry_date { get; set; }
        public bool f_is_active { get; set; } = true;
    }
}
