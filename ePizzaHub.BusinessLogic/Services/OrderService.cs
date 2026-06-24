using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersAsync()
        {
            return await _orderRepository.Get();
        }

        public async Task<OrderModel?> GetOrderByUidAsync(Guid uid)
        {
            return await _orderRepository.Find(uid);
        }

        public async Task<GridDataModel<OrderModel>> GetOrdersExtendedForGridAsync(FilterModel filter)
        {
            return await _orderRepository.GetExtendedForGrid(filter);
        }

        public async Task<OrderModel> CreateOrderAsync(OrderModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _orderRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderModel> UpdateOrderAsync(OrderModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _orderRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteOrderAsync(Guid uid)
        {
            try
            {
                await _unitOfWork.BeginAsync();

                var targetOrder = await _orderRepository.Find(uid);
                if (targetOrder == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return 0;
                }

                await _orderRepository.Remove(targetOrder);
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