using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<RoleModel>> GetRolesAsync()
        {
            return _roleRepository.Get();
        }

        public Task<RoleModel?> GetRoleByUidAsync(Guid uid)
        {
            return _roleRepository.Find(uid);
        }

        public async Task<RoleModel> CreateRoleAsync(RoleModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _roleRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<RoleModel> UpdateRoleAsync(RoleModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _roleRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteRoleAsync(Guid uid)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var targetRole = await _roleRepository.Find(uid);

                if (targetRole == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return 0;
                }

                await _roleRepository.Remove(targetRole);
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