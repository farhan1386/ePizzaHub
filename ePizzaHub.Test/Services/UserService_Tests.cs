using Moq;
using Xunit;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;

namespace ePizzaHub.Test.Services
{
    public class UserService_Tests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly UserService _service;

        public UserService_Tests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();

            _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
            _mockUnitOfWork.Setup(u => u.Roles).Returns(_mockRoleRepo.Object);

            _service = new UserService(_mockUnitOfWork.Object);
        }

        private UserModel CreateDummyUser(string email, string plainPassword)
        {
            return new UserModel
            {
                f_fname = "John",
                f_lname = "Doe",
                f_phone = "123-456-7890",
                f_email = email,
                f_password_hash = plainPassword,
                f_is_active = true
            };
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnNull_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = CreateDummyUser("duplicate@example.com", "anyHash");
            var newUser = CreateDummyUser("DUPLICATE@example.com", "plainPass");

            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel> { existingUser });

            // Act
            var result = await _service.RegisterUserAsync(newUser);

            // Assert
            Assert.Null(result);
            _mockUserRepo.Verify(r => r.Add(It.IsAny<UserModel>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldHashPasswordAndAssignRole_OnSuccess()
        {
            // Arrange
            var newUser = CreateDummyUser("newuser@example.com", "securePassword123");
            var customerRoleUid = Guid.NewGuid();
            var systemRoles = new List<RoleModel>
            {
                new RoleModel { f_uid = customerRoleUid, f_name = "Customer" }
            };

            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel>());
            _mockRoleRepo.Setup(r => r.Get()).ReturnsAsync(systemRoles);

            // Act
            var result = await _service.RegisterUserAsync(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.f_is_active);
            Assert.Equal(customerRoleUid, result.f_role_uid);

            // Verify BCrypt hash operation altered the password
            Assert.NotEqual("securePassword123", result.f_password_hash);
            Assert.True(BCrypt.Net.BCrypt.Verify("securePassword123", result.f_password_hash));

            _mockUserRepo.Verify(r => r.Add(result), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel>());

            // Act
            var result = await _service.AuthenticateUserAsync("missing@example.com", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnNull_WhenUserIsInactive()
        {
            // Arrange
            var inactiveUser = CreateDummyUser("inactive@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
            inactiveUser.f_is_active = false;

            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel> { inactiveUser });

            // Act
            var result = await _service.AuthenticateUserAsync("inactive@example.com", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var correctHash = BCrypt.Net.BCrypt.HashPassword("correctPassword");
            var user = CreateDummyUser("user@example.com", correctHash);
            user.f_is_active = true;

            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel> { user });

            // Act
            var result = await _service.AuthenticateUserAsync("user@example.com", "wrongPassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var correctHash = BCrypt.Net.BCrypt.HashPassword("validPassword");
            var user = CreateDummyUser("user@example.com", correctHash);
            user.f_is_active = true;

            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel> { user });

            // Act
            var result = await _service.AuthenticateUserAsync("USER@example.com", "validPassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.f_email, result.f_email);
        }

        [Fact]
        public async Task GeneratePasswordResetTokenAsync_ShouldReturnFalse_WhenEmailNotFound()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel>());

            // Act
            var result = await _service.GeneratePasswordResetTokenAsync("unknown@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GeneratePasswordResetTokenAsync_ShouldReturnTrue_WhenEmailMatches()
        {
            // Arrange
            var user = CreateDummyUser("exists@example.com", "someHash");
            _mockUserRepo.Setup(r => r.Get()).ReturnsAsync(new List<UserModel> { user });

            // Act
            var result = await _service.GeneratePasswordResetTokenAsync("EXISTS@example.com");

            // Assert
            Assert.True(result);
        }
    }
}