using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserModel?> RegisterUserAsync(UserModel model)
        {
            var existingUsers = await _unitOfWork.Users.Get();
            if (existingUsers.Any(u => u.f_email.Equals(model.f_email, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            model.f_password_hash = BCrypt.Net.BCrypt.HashPassword(model.f_password_hash);
            model.f_is_active = true;

            var roles = await _unitOfWork.Roles.Get();
            var customerRole = roles.FirstOrDefault(r => r.f_name.Equals("Customer", StringComparison.OrdinalIgnoreCase));
            if (customerRole != null)
            {
                model.f_role_uid = customerRole.f_uid;
            }

            await _unitOfWork.Users.Add(model);
            await _unitOfWork.CompleteAsync();
            return model;
        }

        public async Task<UserModel?> AuthenticateUserAsync(string email, string rawPassword)
        {
            var users = await _unitOfWork.Users.Get();
            var match = users.FirstOrDefault(u => u.f_email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (match == null || !match.f_is_active) return null;

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(rawPassword, match.f_password_hash);
            if (!isPasswordCorrect) return null;

            return match;
        }

        public async Task<bool> GeneratePasswordResetTokenAsync(string email)
        {
            var users = await _unitOfWork.Users.Get();
            var match = users.FirstOrDefault(u => u.f_email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (match == null) return false;

            return true;
        }
    }
}
