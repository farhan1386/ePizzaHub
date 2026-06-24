using System;

namespace ePizzaHub.Models
{
    public class OrderItemModel : BaseModel
    {
        public Guid f_order_uid { get; set; }
        public Guid f_pizza_uid { get; set; }
        public int f_quantity { get; set; }
        public decimal f_unit_price { get; set; }
        public OrderModel? f_order { get; set; }
        public PizzaModel? f_pizza { get; set; }
    }
}
