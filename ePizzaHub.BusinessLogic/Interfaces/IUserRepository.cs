using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> Get();
        Task<UserModel?> Find(Guid uid);
        Task<GridDataModel<UserModel>> GetExtendedForGrid(FilterModel filter);
        Task<UserModel> Add(UserModel model);
        Task<UserModel> Update(UserModel model);
        Task<int> Remove(UserModel model);
    }
}
