using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class CartItemModel : BaseModel
    {
        [Required]
        public required string f_customer_session_uid { get; set; }
        public Guid f_pizza_uid { get; set; }
        public int f_quantity { get; set; }
        public PizzaModel? f_pizza { get; set; }
    }
}