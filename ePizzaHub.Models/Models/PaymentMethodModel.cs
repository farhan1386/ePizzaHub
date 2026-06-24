using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ePizzaHub.Models
{
    public class PaymentMethodModel : BaseModel
    {
        [Required]
        public required string f_name { get; set; } // e.g., Stripe, PayPal, Cash On Delivery

        public bool f_is_active { get; set; } = true;
    }
}
