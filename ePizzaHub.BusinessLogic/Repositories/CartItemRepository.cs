using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public CartItemRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<CartItemModel>> Get()
        {
            return await _context.CartItems
                .Include(c => c.f_pizza)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CartItemModel?> Find(Guid uid)
        {
            return await _context.CartItems
                .Include(c => c.f_pizza)
                .FirstOrDefaultAsync(c => c.f_uid == uid);
        }

        public async Task<GridDataModel<CartItemModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.CartItems
                .Include(c => c.f_pizza)
                .AsNoTracking()
                .AsQueryable();

            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.f_customer_session_uid.ToLower().Contains(search) ||
                                         (c.f_pizza != null && c.f_pizza.f_name.ToLower().Contains(search)));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_quantity" => isDesc ? query.OrderByDescending(c => c.f_quantity) : query.OrderBy(c => c.f_quantity),
                    "f_customer_session_uid" => isDesc ? query.OrderByDescending(c => c.f_customer_session_uid) : query.OrderBy(c => c.f_customer_session_uid),
                    _ => query.OrderByDescending(c => c.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<CartItemModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<CartItemModel> Add(CartItemModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            await _context.CartItems.AddAsync(model);
            return model;
        }

        public async Task<CartItemModel> Update(CartItemModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            _context.CartItems.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(CartItemModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            _context.CartItems.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}