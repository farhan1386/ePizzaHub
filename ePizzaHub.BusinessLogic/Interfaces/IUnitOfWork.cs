using System;
using System.Threading.Tasks;

namespace ePizzaHub.BusinessLogic.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICartItemRepository CartItemRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPizzaRepository Pizzas { get; }
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        Task<int> CompleteAsync();
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}