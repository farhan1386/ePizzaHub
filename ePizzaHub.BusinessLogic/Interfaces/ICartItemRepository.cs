using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItemModel>> Get();
        Task<CartItemModel?> Find(Guid uid);
        Task<GridDataModel<CartItemModel>> GetExtendedForGrid(FilterModel filter);
        Task<CartItemModel> Add(CartItemModel model);
        Task<CartItemModel> Update(CartItemModel model);
        Task<int> Remove(CartItemModel model);
    }
}
