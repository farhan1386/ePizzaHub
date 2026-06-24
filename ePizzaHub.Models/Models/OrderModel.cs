using ePizzaHub.Models.Enums;

namespace ePizzaHub.Models
{
    public class OrderModel : BaseModel
    {
        public required string f_customer_user_uid { get; set; }
        public decimal f_total_amount { get; set; }
        public required string f_delivery_address { get; set; }
        public required string f_contact_phone { get; set; }
        public OrderStatus f_order_status { get; set; }
        public bool f_is_paid { get; set; } = false;
        public List<OrderItemModel> f_order_items { get; set; } = [];
    }
}
