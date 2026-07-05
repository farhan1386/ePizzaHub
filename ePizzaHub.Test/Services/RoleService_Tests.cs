using Moq;
using Xunit;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;

namespace ePizzaHub.Test.Services
{
    public class RoleService_Tests
    {
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly RoleService _service;

        public RoleService_Tests()
        {
            _mockRepo = new Mock<IRoleRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new RoleService(_mockRepo.Object, _mockUnitOfWork.Object);
        }

        private RoleModel CreateDummyRole()
        {
            return new RoleModel
            {
                f_name = "Admin",
                f_is_active = true
            };
        }

        [Fact]
        public async Task GetRolesAsync_ShouldReturnAllRoles()
        {
            // Arrange
            var expectedRoles = new List<RoleModel> { CreateDummyRole(), CreateDummyRole() };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(expectedRoles);

            // Act
            var result = await _service.GetRolesAsync();

            // Assert
            Assert.Equal(expectedRoles, result);
            _mockRepo.Verify(r => r.Get(), Times.Once);
        }

        [Fact]
        public async Task GetRoleByUidAsync_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var expectedRole = CreateDummyRole();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(expectedRole);

            // Act
            var result = await _service.GetRoleByUidAsync(targetUid);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Fact]
        public async Task CreateRoleAsync_ShouldCommitAndReturnRole_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyRole();
            var createdModel = CreateDummyRole();
            _mockRepo.Setup(r => r.Add(inputModel)).ReturnsAsync(createdModel);

            // Act
            var result = await _service.CreateRoleAsync(inputModel);

            // Assert
            Assert.Equal(createdModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateRoleAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyRole();
            _mockRepo.Setup(r => r.Add(inputModel)).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateRoleAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldCommitAndReturnRole_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyRole();
            var updatedModel = CreateDummyRole();
            _mockRepo.Setup(r => r.Update(inputModel)).ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdateRoleAsync(inputModel);

            // Assert
            Assert.Equal(updatedModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyRole();
            _mockRepo.Setup(r => r.Update(inputModel)).ThrowsAsync(new Exception("Update Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateRoleAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldReturnChanges_WhenRoleExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var targetRole = CreateDummyRole();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(targetRole);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteRoleAsync(targetUid);

            // Assert
            Assert.Equal(1, result);
            _mockRepo.Verify(r => r.Remove(targetRole), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldRollbackAndReturnZero_WhenRoleNotFound()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync((RoleModel?)null);

            // Act
            var result = await _service.DeleteRoleAsync(targetUid);

            // Assert
            Assert.Equal(0, result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<RoleModel>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ThrowsAsync(new Exception("Delete Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteRoleAsync(targetUid));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}