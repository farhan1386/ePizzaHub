using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Moq;

namespace ePizzaHub.Test.Services
{
    public class PizzaService_Tests
    {
        private readonly Mock<IPizzaRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PizzaService _service;

        public PizzaService_Tests()
        {
            _mockRepo = new Mock<IPizzaRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new PizzaService(_mockRepo.Object, _mockUnitOfWork.Object);
        }

        private PizzaModel CreateDummyPizza()
        {
            return new PizzaModel
            {
                f_name = "Margherita",
                f_description = "Classic cheese and tomato",
                f_image_url = "http://example.com",
                f_price = 199.00m,
                f_is_available = true
            };
        }

        [Fact]
        public async Task GetPizzaAsync_ShouldReturnAllPizzas()
        {
            // Arrange
            var expectedPizzas = new List<PizzaModel> { CreateDummyPizza(), CreateDummyPizza() };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(expectedPizzas);

            // Act
            var result = await _service.GetPizzaAsync();

            // Assert
            Assert.Equal(expectedPizzas, result);
            _mockRepo.Verify(r => r.Get(), Times.Once);
        }

        [Fact]
        public async Task GetPizzaByUidAsync_ShouldReturnPizza_WhenPizzaExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var expectedPizza = CreateDummyPizza();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(expectedPizza);

            // Act
            var result = await _service.GetPizzaByUidAsync(targetUid);

            // Assert
            Assert.Equal(expectedPizza, result);
        }

        [Fact]
        public async Task GetPizzaExtendedForGridAsync_ShouldReturnGridData()
        {
            // Arrange
            var filter = new FilterModel();
            var expectedGrid = new GridDataModel<PizzaModel>();
            _mockRepo.Setup(r => r.GetExtendedForGrid(filter)).ReturnsAsync(expectedGrid);

            // Act
            var result = await _service.GetPizzaExtendedForGridAsync(filter);

            // Assert
            Assert.Equal(expectedGrid, result);
        }

        [Fact]
        public async Task CreatePizzaAsync_ShouldCommitAndReturnPizza_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyPizza();
            var createdModel = CreateDummyPizza();
            _mockRepo.Setup(r => r.Add(inputModel)).ReturnsAsync(createdModel);

            // Act
            var result = await _service.CreatePizzaAsync(inputModel);

            // Assert
            Assert.Equal(createdModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task CreatePizzaAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyPizza();
            _mockRepo.Setup(r => r.Add(inputModel)).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreatePizzaAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePizzaAsync_ShouldCommitAndReturnPizza_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyPizza();
            var updatedModel = CreateDummyPizza();
            _mockRepo.Setup(r => r.Update(inputModel)).ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdatePizzaAsync(inputModel);

            // Assert
            Assert.Equal(updatedModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePizzaAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyPizza();
            _mockRepo.Setup(r => r.Update(inputModel)).ThrowsAsync(new Exception("Update Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdatePizzaAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePizzaAsync_ShouldReturnChanges_WhenPizzaExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var targetPizza = CreateDummyPizza();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(targetPizza);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeletePizzaAsync(targetUid);

            // Assert
            Assert.Equal(1, result);
            _mockRepo.Verify(r => r.Remove(targetPizza), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeletePizzaAsync_ShouldRollbackAndReturnZero_WhenPizzaNotFound()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync((PizzaModel?)null);

            // Act
            var result = await _service.DeletePizzaAsync(targetUid);

            // Assert
            Assert.Equal(0, result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<PizzaModel>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeletePizzaAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ThrowsAsync(new Exception("Delete Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeletePizzaAsync(targetUid));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}