using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class CartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartItemService(ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CartItemModel>> GetCartItemsAsync()
        {
            return await _cartItemRepository.Get();
        }

        public async Task<CartItemModel?> GetCartItemByUidAsync(Guid uid)
        {
            return await _cartItemRepository.Find(uid);
        }

        public async Task<GridDataModel<CartItemModel>> GetCartItemsExtendedForGridAsync(FilterModel filter)
        {
            return await _cartItemRepository.GetExtendedForGrid(filter);
        }

        public async Task<CartItemModel> CreateCartItemAsync(CartItemModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _cartItemRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<CartItemModel> UpdateCartItemAsync(CartItemModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _cartItemRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteCartItemAsync(Guid uid)
        {
            try
            {
                await _unitOfWork.BeginAsync();

                var targetItem = await _cartItemRepository.Find(uid);
                if (targetItem == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return 0;
                }

                await _cartItemRepository.Remove(targetItem);
                var changes = await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitAsync();
                return changes;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}