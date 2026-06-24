using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;
        private IDbContextTransaction? _currentTransaction;
        private IPizzaRepository? _pizzas;

        public UnitOfWork(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _invokeData = invokeData ?? throw new ArgumentNullException(nameof(invokeData));
        }

        public ICartItemRepository CartItemRepository => new CartItemRepository(_context, _invokeData);

        public IOrderItemRepository OrderItemRepository => new OrderItemRepository(_context, _invokeData);

        public IOrderRepository OrderRepository => new OrderRepository(_context, _invokeData);
        public IPizzaRepository Pizzas => _pizzas ??= new PizzaRepository(_context, _invokeData);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginAsync()
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}