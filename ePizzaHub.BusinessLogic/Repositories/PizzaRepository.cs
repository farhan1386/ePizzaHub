using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class PizzaRepository : IPizzaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public PizzaRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<PizzaModel>> Get()
        {
            return await _context.Pizzas.AsNoTracking().ToListAsync();
        }

        public async Task<PizzaModel?> Find(Guid uid)
        {
            return await _context.Pizzas.FirstOrDefaultAsync(p => p.f_uid == uid);
        }

        public async Task<GridDataModel<PizzaModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.Pizzas.AsNoTracking().AsQueryable();
            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.f_name.ToLower().Contains(search) ||
                                         p.f_description.ToLower().Contains(search));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_name" => isDesc ? query.OrderByDescending(p => p.f_name) : query.OrderBy(p => p.f_name),
                    "f_price" => isDesc ? query.OrderByDescending(p => p.f_price) : query.OrderBy(p => p.f_price),
                    _ => query.OrderByDescending(p => p.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<PizzaModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<PizzaModel> Add(PizzaModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            await _context.Pizzas.AddAsync(model);
            return model;
        }

        public async Task<PizzaModel> Update(PizzaModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            _context.Pizzas.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(PizzaModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            _context.Pizzas.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}
