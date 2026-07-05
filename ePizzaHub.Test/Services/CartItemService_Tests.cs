using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Moq;

namespace ePizzaHub.Test.Services
{
    public class CartItemService_Tests
    {
        private readonly Mock<ICartItemRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CartItemService _service;
        private const string DummySessionUid = "dummy-session-id";

        public CartItemService_Tests()
        {
            _mockRepo = new Mock<ICartItemRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new CartItemService(_mockRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetCartItemsAsync_ShouldReturnAllItems()
        {
            // Arrange
            var expectedItems = new List<CartItemModel>
            {
                new CartItemModel { f_customer_session_uid = DummySessionUid },
                new CartItemModel { f_customer_session_uid = DummySessionUid }
            };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(expectedItems);

            // Act
            var result = await _service.GetCartItemsAsync();

            // Assert
            Assert.Equal(expectedItems, result);
            _mockRepo.Verify(r => r.Get(), Times.Once);
        }

        [Fact]
        public async Task GetCartItemsBySessionAsync_ShouldFilterBySessionUid()
        {
            // Arrange
            string targetSession = "session-123";
            var allItems = new List<CartItemModel>
            {
                new CartItemModel { f_customer_session_uid = "session-123" },
                new CartItemModel { f_customer_session_uid = "session-456" },
                new CartItemModel { f_customer_session_uid = "session-123" }
            };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(allItems);

            // Act
            var result = await _service.GetCartItemsBySessionAsync(targetSession);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(targetSession, item.f_customer_session_uid));
        }

        [Fact]
        public async Task GetCartItemByUidAsync_ShouldReturnItem_WhenItemExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var expectedItem = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(expectedItem);

            // Act
            var result = await _service.GetCartItemByUidAsync(targetUid);

            // Assert
            Assert.Equal(expectedItem, result);
        }

        [Fact]
        public async Task GetCartItemsExtendedForGridAsync_ShouldReturnGridData()
        {
            // Arrange
            var filter = new FilterModel();
            var expectedGrid = new GridDataModel<CartItemModel>();
            _mockRepo.Setup(r => r.GetExtendedForGrid(filter)).ReturnsAsync(expectedGrid);

            // Act
            var result = await _service.GetCartItemsExtendedForGridAsync(filter);

            // Assert
            Assert.Equal(expectedGrid, result);
        }

        [Fact]
        public async Task CreateCartItemAsync_ShouldCommitAndReturnItem_OnSuccess()
        {
            // Arrange
            var inputModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            var createdModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Add(inputModel)).ReturnsAsync(createdModel);

            // Act
            var result = await _service.CreateCartItemAsync(inputModel);

            // Assert
            Assert.Equal(createdModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateCartItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Add(inputModel)).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateCartItemAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCartItemAsync_ShouldCommitAndReturnItem_OnSuccess()
        {
            // Arrange
            var inputModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            var updatedModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Update(inputModel)).ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdateCartItemAsync(inputModel);

            // Assert
            Assert.Equal(updatedModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateCartItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Update(inputModel)).ThrowsAsync(new Exception("Update Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateCartItemAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItemAsync_ShouldReturnChanges_WhenItemExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var targetItem = new CartItemModel { f_customer_session_uid = DummySessionUid };
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(targetItem);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteCartItemAsync(targetUid);

            // Assert
            Assert.Equal(1, result);
            _mockRepo.Verify(r => r.Remove(targetItem), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCartItemAsync_ShouldRollbackAndReturnZero_WhenItemNotFound()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync((CartItemModel?)null);

            // Act
            var result = await _service.DeleteCartItemAsync(targetUid);

            // Assert
            Assert.Equal(0, result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<CartItemModel>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCartItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ThrowsAsync(new Exception("Delete Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteCartItemAsync(targetUid));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
