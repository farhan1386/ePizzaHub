using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using ePizzaHub.Models.Enums;
using Moq;

namespace ePizzaHub.Test.Services
{
    public class OrderService_Tests
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly OrderService _service;

        public OrderService_Tests()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new OrderService(_mockRepo.Object, _mockUnitOfWork.Object);
        }

        private OrderModel CreateDummyOrder()
        {
            return new OrderModel
            {
                f_customer_user_uid = "user-123",
                f_delivery_address = "123 Main St",
                f_contact_phone = "555-0199",
                f_order_status = OrderStatus.Pending,
                f_is_paid = false,
                f_total_amount = 0,
                f_order_items = new List<OrderItemModel>()
            };
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var expectedOrders = new List<OrderModel> { CreateDummyOrder(), CreateDummyOrder() };
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(expectedOrders);

            // Act
            var result = await _service.GetOrdersAsync();

            // Assert
            Assert.Equal(expectedOrders, result);
            _mockRepo.Verify(r => r.Get(), Times.Once);
        }

        [Fact]
        public async Task GetOrderByUidAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var expectedOrder = CreateDummyOrder();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(expectedOrder);

            // Act
            var result = await _service.GetOrderByUidAsync(targetUid);

            // Assert
            Assert.Equal(expectedOrder, result);
        }

        [Fact]
        public async Task GetOrdersExtendedForGridAsync_ShouldReturnGridData()
        {
            // Arrange
            var filter = new FilterModel();
            var expectedGrid = new GridDataModel<OrderModel>();
            _mockRepo.Setup(r => r.GetExtendedForGrid(filter)).ReturnsAsync(expectedGrid);

            // Act
            var result = await _service.GetOrdersExtendedForGridAsync(filter);

            // Assert
            Assert.Equal(expectedGrid, result);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCommitAndReturnOrder_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyOrder();
            var createdModel = CreateDummyOrder();
            _mockRepo.Setup(r => r.Add(inputModel)).ReturnsAsync(createdModel);

            // Act
            var result = await _service.CreateOrderAsync(inputModel);

            // Assert
            Assert.Equal(createdModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyOrder();
            _mockRepo.Setup(r => r.Add(inputModel)).ThrowsAsync(new Exception("Database Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldCommitAndReturnOrder_OnSuccess()
        {
            // Arrange
            var inputModel = CreateDummyOrder();
            var updatedModel = CreateDummyOrder();
            _mockRepo.Setup(r => r.Update(inputModel)).ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdateOrderAsync(inputModel);

            // Assert
            Assert.Equal(updatedModel, result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var inputModel = CreateDummyOrder();
            _mockRepo.Setup(r => r.Update(inputModel)).ThrowsAsync(new Exception("Update Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateOrderAsync(inputModel));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnChanges_WhenOrderExists()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            var targetOrder = CreateDummyOrder();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync(targetOrder);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteOrderAsync(targetUid);

            // Assert
            Assert.Equal(1, result);
            _mockRepo.Verify(r => r.Remove(targetOrder), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldRollbackAndReturnZero_WhenOrderNotFound()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ReturnsAsync((OrderModel?)null);

            // Act
            var result = await _service.DeleteOrderAsync(targetUid);

            // Assert
            Assert.Equal(0, result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<OrderModel>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var targetUid = Guid.NewGuid();
            _mockRepo.Setup(r => r.Find(targetUid)).ThrowsAsync(new Exception("Delete Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteOrderAsync(targetUid));
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}