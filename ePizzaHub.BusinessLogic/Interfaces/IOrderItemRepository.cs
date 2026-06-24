using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItemModel>> Get();
        Task<OrderItemModel?> Find(Guid uid);
        Task<GridDataModel<OrderItemModel>> GetExtendedForGrid(FilterModel filter);
        Task<OrderItemModel> Add(OrderItemModel model);
        Task<OrderItemModel> Update(OrderItemModel model);
        Task<int> Remove(OrderItemModel model);
    }
}
