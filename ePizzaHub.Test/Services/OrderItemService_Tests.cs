using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Moq;

namespace ePizzaHub.Test.Services
{
    public class OrderItemService_Tests
    {
        private readonly Mock<IOrderItemRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly OrderItemService _service;

        public OrderItemService_Tests()
        {
            _mockRepo = new Mock<IOrderItemRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new OrderItemService(_mockRepo.Object, _mockUnitOfWork.Object);
        }

        private OrderItemModel CreateDummyOrderItem()
        {
            return new OrderItemModel
            {
                f_order_uid = Guid.NewGuid(),
                f_pizza_uid = Guid.NewGuid(),
                f_quantity = 1,
                f_unit_price = 150
            };
        }

        [Fact]
        public async Task GetOrderItemsAsync_ShouldReturnAllItems()
        {
            // Arrange
            var expectedItems = new List<OrderItemModel> { CreateDummyOrderItem(), CreateDummyOrderItem() };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(expectedItems);

            // Act
            var result = await _service.GetOrderItemsAsync();

            // Assert
            Assert.Equal(expectedItems, result);
            _mockRepo.Verify(r => r.Get(), Times.Once);
        }

        [Fact]
        public async Task GetOrderItemByUidAsync_ShouldReturnItem_WhenItemExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var expectedItem = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(expectedItem);

            // Act
            var result = await _service.GetOrderItemByUidAsync(targetUid);

            // Assert
            Assert.Equal(expectedItem, result);
        }

        [Fact]
        public async Task GetOrderItemsExtendedForGridAsync_ShouldReturnGridData()
        {
            // Arrange
            var filter = new FilterModel();
            var expectedGrid = new GridDataModel<OrderItemModel>();
            _mockRepo.Setup(r => r.GetExtendedForGrid(filter)).ReturnsAsync(expectedGrid);

            // Act
            var result = await _service.GetOrderItemsExtendedForGridAsync(filter);

            // Assert
            Assert.Equal(expectedGrid, result);
        }

        [Fact]
        public async Task CreateOrderItemAsync_ShouldCommitAndReturnItem_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyOrderItem();
            var createdModel = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Add(inputModel)).ReturnsAsync(createdModel);

            // Act
            var result = await _service.CreateOrderItemAsync(inputModel);

            // Assert
            Assert.Equal(createdModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateOrderItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Add(inputModel)).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderItemAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderItemAsync_ShouldCommitAndReturnItem_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyOrderItem();
            var updatedModel = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Update(inputModel)).ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdateOrderItemAsync(inputModel);

            // Assert
            Assert.Equal(updatedModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Update(inputModel)).ThrowsAsync(new Exception("Update Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateOrderItemAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderItemAsync_ShouldReturnChanges_WhenItemExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var targetItem = CreateDummyOrderItem();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(targetItem);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteOrderItemAsync(targetUid);

            // Assert
            Assert.Equal(1, result);
            _mockRepo.Verify(r => r.Remove(targetItem), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderItemAsync_ShouldRollbackAndReturnZero_WhenItemNotFound()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync((OrderItemModel?)null);

            // Act
            var result = await _service.DeleteOrderItemAsync(targetUid);

            // Assert
            Assert.Equal(0, result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<OrderItemModel>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderItemAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ThrowsAsync(new Exception("Delete Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteOrderItemAsync(targetUid));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}