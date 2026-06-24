using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public OrderItemRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<OrderItemModel>> Get()
        {
            return await _context.OrderItems
                .Include(oi => oi.f_pizza)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrderItemModel?> Find(Guid uid)
        {
            return await _context.OrderItems
                .Include(oi => oi.f_pizza)
                .FirstOrDefaultAsync(oi => oi.f_uid == uid);
        }

        public async Task<GridDataModel<OrderItemModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.OrderItems
                .Include(oi => oi.f_pizza)
                .AsNoTracking()
                .AsQueryable();

            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(oi => (oi.f_pizza != null && oi.f_pizza.f_name.ToLower().Contains(search)));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_quantity" => isDesc ? query.OrderByDescending(oi => oi.f_quantity) : query.OrderBy(oi => oi.f_quantity),
                    "f_unit_price" => isDesc ? query.OrderByDescending(oi => oi.f_unit_price) : query.OrderBy(oi => oi.f_unit_price),
                    _ => query.OrderByDescending(oi => oi.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(oi => oi.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<OrderItemModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<OrderItemModel> Add(OrderItemModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            await _context.OrderItems.AddAsync(model);
            return model;
        }

        public async Task<OrderItemModel> Update(OrderItemModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            _context.OrderItems.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(OrderItemModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            _context.OrderItems.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}