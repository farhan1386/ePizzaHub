using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public RoleRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<RoleModel>> Get()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<RoleModel?> Find(Guid uid)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.f_uid == uid);
        }

        public async Task<GridDataModel<RoleModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.Roles.AsNoTracking().AsQueryable();

            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(r => r.f_name.ToLower().Contains(search));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_name" => isDesc ? query.OrderByDescending(r => r.f_name) : query.OrderBy(r => r.f_name),
                    _ => query.OrderByDescending(r => r.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(r => r.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<RoleModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<RoleModel> Add(RoleModel model)
        {
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            await _context.Roles.AddAsync(model);
            return model;
        }

        public async Task<RoleModel> Update(RoleModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            _context.Roles.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(RoleModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            _context.Roles.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}