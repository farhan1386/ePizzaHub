using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class OrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemService(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderItemModel>> GetOrderItemsAsync()
        {
            return await _orderItemRepository.Get();
        }

        public async Task<OrderItemModel?> GetOrderItemByUidAsync(Guid uid)
        {
            return await _orderItemRepository.Find(uid);
        }

        public async Task<GridDataModel<OrderItemModel>> GetOrderItemsExtendedForGridAsync(FilterModel filter)
        {
            return await _orderItemRepository.GetExtendedForGrid(filter);
        }

        public async Task<OrderItemModel> CreateOrderItemAsync(OrderItemModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _orderItemRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderItemModel> UpdateOrderItemAsync(OrderItemModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _orderItemRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteOrderItemAsync(Guid uid)
        {
            try
            {
                await _unitOfWork.BeginAsync();

                var targetItem = await _orderItemRepository.Find(uid);
                if (targetItem == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return 0;
                }

                await _orderItemRepository.Remove(targetItem);
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