using ePizzaHub.BusinessLogic.Data;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.BusinessLogic.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly InvokeDataModel _invokeData;

        public UserRepository(ApplicationDbContext context, InvokeDataModel invokeData)
        {
            _context = context;
            _invokeData = invokeData;

            if (_invokeData.commandTimeout.HasValue)
            {
                _context.Database.SetCommandTimeout(_invokeData.commandTimeout.Value);
            }
        }

        public async Task<IEnumerable<UserModel>> Get()
        {
            return await _context.Users
                .Include(u => u.f_role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserModel?> Find(Guid uid)
        {
            return await _context.Users
                .Include(u => u.f_role)
                .FirstOrDefaultAsync(u => u.f_uid == uid);
        }

        public async Task<GridDataModel<UserModel>> GetExtendedForGrid(FilterModel filter)
        {
            var query = _context.Users
                .Include(u => u.f_role)
                .AsNoTracking()
                .AsQueryable();

            int totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(u => u.f_fname.ToLower().Contains(search) ||
                                         u.f_lname.ToLower().Contains(search) ||
                                         u.f_email.ToLower().Contains(search) ||
                                         u.f_phone.Contains(search));
            }

            int filteredRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortColumn))
            {
                var isDesc = filter.SortDirection?.ToUpper() == "DESC";
                query = filter.SortColumn.ToLower() switch
                {
                    "f_fname" => isDesc ? query.OrderByDescending(u => u.f_fname) : query.OrderBy(u => u.f_fname),
                    "f_lname" => isDesc ? query.OrderByDescending(u => u.f_lname) : query.OrderBy(u => u.f_lname),
                    "f_email" => isDesc ? query.OrderByDescending(u => u.f_email) : query.OrderBy(u => u.f_email),
                    _ => query.OrderByDescending(u => u.f_iid)
                };
            }
            else
            {
                query = query.OrderByDescending(u => u.f_iid);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridDataModel<UserModel>
            {
                Data = items,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }

        public async Task<UserModel> Add(UserModel model)
        {
            model.f_create_date = DateTime.Now;
            model.f_create_by = _invokeData.userUid;

            await _context.Users.AddAsync(model);
            return model;
        }

        public async Task<UserModel> Update(UserModel model)
        {
            model.f_update_date = DateTime.Now;
            model.f_update_by = _invokeData.userUid;

            _context.Users.Update(model);
            await Task.CompletedTask;
            return model;
        }

        public async Task<int> Remove(UserModel model)
        {
            model.f_delete_date = DateTime.Now;
            model.f_delete_by = _invokeData.userUid;

            _context.Users.Update(model);
            await Task.CompletedTask;
            return 1;
        }
    }
}