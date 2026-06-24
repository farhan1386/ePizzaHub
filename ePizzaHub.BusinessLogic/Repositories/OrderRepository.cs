using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public OrderRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<OrderModel>> Get()
        {
            return await _context.Orders
                .Include(o => o.f_order_items)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrderModel?> Find(Guid uid)
        {
            return await _context.Orders
                .Include(o => o.f_order_items)
                .FirstOrDefaultAsync(o => o.f_uid == uid);
        }

        public async Task<GridDataModel<OrderModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.Orders
                .Include(o => o.f_order_items)
                .AsNoTracking()
                .AsQueryable();

            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(o => o.f_delivery_address.ToLower().Contains(search) ||
                                         o.f_contact_phone.ToLower().Contains(search) ||
                                         o.f_customer_user_uid.ToLower().Contains(search));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_total_amount" => isDesc ? query.OrderByDescending(o => o.f_total_amount) : query.OrderBy(o => o.f_total_amount),
                    "f_customer_user_uid" => isDesc ? query.OrderByDescending(o => o.f_customer_user_uid) : query.OrderBy(o => o.f_customer_user_uid),
                    _ => query.OrderByDescending(o => o.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(o => o.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<OrderModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<OrderModel> Add(OrderModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            foreach (var item in model.f_order_items)
            {
                item.f_uid = Guid.NewGuid();
                item.f_create_date = DateTime.Now;
                item.f_create_by = _invokeData.userUid;
            }

            await _context.Orders.AddAsync(model);
            return model;
        }

        public async Task<OrderModel> Update(OrderModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            foreach (var item in model.f_order_items)
            {
                item.f_update_date = DateTime.Now;
                item.f_update_by = _invokeData.userUid;
            }

            _context.Orders.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(OrderModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            foreach (var item in model.f_order_items)
            {
                item.f_delete_date = DateTime.Now;
                item.f_delete_by = _invokeData.userUid;
            }

            _context.Orders.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}