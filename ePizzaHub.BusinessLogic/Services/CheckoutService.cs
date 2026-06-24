using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using ePizzaHub.Models.Enums;

namespace ePizzaHub.BusinessLogic.Services
{
    public class CheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CartItemService _cartItemService;

        public CheckoutService(IUnitOfWork unitOfWork, CartItemService cartItemService)
        {
            _unitOfWork = unitOfWork;
            _cartItemService = cartItemService;
        }

        public async Task<OrderModel?> ProcessCheckoutAsync(string sessionUid, string userUid, string address, string phone)
        {
            var allCartItems = await _cartItemService.GetCartItemsAsync();
            var cartItems = allCartItems.Where(c => c.f_customer_session_uid == sessionUid).ToList();
            if (!cartItems.Any()) return null;

            try
            {
                await _unitOfWork.BeginAsync();

                var order = new OrderModel
                {
                    f_customer_user_uid = userUid,
                    f_delivery_address = address,
                    f_contact_phone = phone,
                    f_order_status = OrderStatus.Pending,
                    f_is_paid = false,
                    f_total_amount = 0,
                    f_order_items = new List<OrderItemModel>()
                };

                var pizzas = await _unitOfWork.Pizzas.Get();

                foreach (var item in cartItems)
                {
                    var matchedPizza = pizzas.FirstOrDefault(p => p.f_uid == item.f_pizza_uid);
                    if (matchedPizza == null) continue;

                    order.f_total_amount += matchedPizza.f_price * item.f_quantity;

                    order.f_order_items.Add(new OrderItemModel
                    {
                        f_order_uid = order.f_uid,
                        f_pizza_uid = item.f_pizza_uid,
                        f_quantity = item.f_quantity,
                        f_unit_price = matchedPizza.f_price
                    });

                    await _unitOfWork.CartItemRepository.Remove(item);
                }

                // FIXED: Changed from .Orders to .OrderRepository
                await _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CommitAsync();
                return order;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
