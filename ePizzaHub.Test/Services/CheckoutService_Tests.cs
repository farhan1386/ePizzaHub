using Moq;
using Xunit;
using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using ePizzaHub.Models.Enums;

namespace ePizzaHub.Test.Services
{
    public class CheckoutService_Tests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICartItemRepository> _mockCartRepo;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<IPizzaRepository> _mockPizzaRepo; // Assumes IPizzaRepository exists under UnitOfWork
        private readonly CartItemService _cartItemService;
        private readonly CheckoutService _service;
        private const string DummySessionUid = "session-123";

        public CheckoutService_Tests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartRepo = new Mock<ICartItemRepository>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockPizzaRepo = new Mock<IPizzaRepository>();

            // Setup UnitOfWork structure to match your code
            _mockUnitOfWork.Setup(u => u.CartItemRepository).Returns(_mockCartRepo.Object);
            _mockUnitOfWork.Setup(u => u.OrderRepository).Returns(_mockOrderRepo.Object);
            _mockUnitOfWork.Setup(u => u.Pizzas).Returns(_mockPizzaRepo.Object);

            // CheckoutService depends on CartItemService. We use the actual service backed by mocked repo.
            _cartItemService = new CartItemService(_mockCartRepo.Object, _mockUnitOfWork.Object);
            _service = new CheckoutService(_mockUnitOfWork.Object, _cartItemService);
        }

        [Fact]
        public async Task ProcessCheckoutAsync_ShouldReturnNull_WhenCartIsEmpty()
        {
            // Arrange
            _mockCartRepo.Setup(r => r.Get()).ReturnsAsync(new List<CartItemModel>());

            // Act
            var result = await _service.ProcessCheckoutAsync(DummySessionUid, "user-1", "Address", "12345");

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessCheckoutAsync_ShouldCreateOrderAndEmptyCart_OnSuccess()
        {
            // Arrange
            var pizzaUid1 = Guid.NewGuid();
            var pizzaUid2 = Guid.NewGuid();

            var cartItems = new List<CartItemModel>
            {
                new CartItemModel { f_customer_session_uid = DummySessionUid, f_pizza_uid = pizzaUid1, f_quantity = 2 },
                new CartItemModel { f_customer_session_uid = DummySessionUid, f_pizza_uid = pizzaUid2, f_quantity = 1 },
                new CartItemModel { f_customer_session_uid = "other-session", f_pizza_uid = pizzaUid1, f_quantity = 5 } // unrelated
            };

            var pizzas = new List<PizzaModel>
            {
                new PizzaModel
                {
                    f_uid = pizzaUid1,
                    f_price = 100,
                    f_name = "Pizza 1",
                    f_description = "Description 1",
                    f_image_url = "http://example.com"
                },
                new PizzaModel
                {
                    f_uid = pizzaUid2,
                    f_price = 150,
                    f_name = "Pizza 2",
                    f_description = "Description 2",
                    f_image_url = "http://example.com"
                }
            };

            _mockCartRepo.Setup(r => r.Get()).ReturnsAsync(cartItems);
            _mockPizzaRepo.Setup(r => r.Get()).ReturnsAsync(pizzas);

            // Act
            var result = await _service.ProcessCheckoutAsync(DummySessionUid, "user-1", "123 Main St", "99999");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-1", result.f_customer_user_uid);
            Assert.Equal("123 Main St", result.f_delivery_address);
            Assert.Equal("99999", result.f_contact_phone);
            Assert.Equal(OrderStatus.Pending, result.f_order_status);
            Assert.False(result.f_is_paid);

            // (100 * 2) + (150 * 1) = 350
            Assert.Equal(350, result.f_total_amount);
            Assert.Equal(2, result.f_order_items.Count);

            // Verify interactions
            _mockCartRepo.Verify(r => r.Remove(It.IsAny<CartItemModel>()), Times.Exactly(2));
            _mockOrderRepo.Verify(r => r.Add(It.IsAny<OrderModel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessCheckoutAsync_ShouldIgnoreCartItems_WhenPizzaNotFound()
        {
            // Arrange
            var unknownPizzaUid = Guid.NewGuid();
            var cartItems = new List<CartItemModel>
            {
                new CartItemModel { f_customer_session_uid = DummySessionUid, f_pizza_uid = unknownPizzaUid, f_quantity = 2 }
            };

            _mockCartRepo.Setup(r => r.Get()).ReturnsAsync(cartItems);
            _mockPizzaRepo.Setup(r => r.Get()).ReturnsAsync(new List<PizzaModel>()); // Empty pizza catalog

            // Act
            var result = await _service.ProcessCheckoutAsync(DummySessionUid, "user-1", "Address", "123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.f_total_amount);
            Assert.Empty(result.f_order_items);
            _mockCartRepo.Verify(r => r.Remove(It.IsAny<CartItemModel>()), Times.Never);
        }

        [Fact]
        public async Task ProcessCheckoutAsync_ShouldRollbackAndThrow_OnException()
        {
            // Arrange
            var cartItems = new List<CartItemModel>
            {
                new CartItemModel { f_customer_session_uid = DummySessionUid, f_pizza_uid = Guid.NewGuid(), f_quantity = 1 }
            };
            _mockCartRepo.Setup(r => r.Get()).ReturnsAsync(cartItems);
            _mockPizzaRepo.Setup(r => r.Get()).ThrowsAsync(new Exception("Database Timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.ProcessCheckoutAsync(DummySessionUid, "user-1", "Address", "123"));

            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}