using ePizzaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ePizzaHub.Models
{
    public class PaymentModel : BaseModel
    {
        public Guid f_order_uid { get; set; }
        public Guid f_payment_method_uid { get; set; }
        [Required]
        public required string f_transaction_id { get; set; }
        public decimal f_amount { get; set; }
        public PaymentStatus f_payment_status { get; set; }
        public OrderModel? f_order { get; set; }
        public PaymentMethodModel? f_payment_method { get; set; }
    }
}
