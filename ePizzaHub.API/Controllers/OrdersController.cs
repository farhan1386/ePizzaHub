using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderModel>>> Get([FromQuery] string? sessionUid = null)
        {
            var allOrders = await _orderService.GetOrdersAsync();

            if (!string.IsNullOrEmpty(sessionUid))
            {
                var filtered = allOrders.Where(o => o.f_customer_user_uid == sessionUid);
                return Ok(filtered);
            }

            return Ok(allOrders);
        }

        [HttpGet("{uid:guid}")]
        public async Task<ActionResult<OrderModel>> Get(Guid uid)
        {
            var result = await _orderService.GetOrderByUidAsync(uid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<OrderModel>> Search(string keyword)
        {
            var allOrders = await _orderService.GetOrdersAsync();
            var match = allOrders.FirstOrDefault(o =>
                o.f_uid.ToString().Equals(keyword, StringComparison.OrdinalIgnoreCase) ||
                o.f_uid.ToString().StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            );

            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridDataModel<OrderModel>>> GetGrid([FromBody] FilterModel filter)
        {
            var result = await _orderService.GetOrdersExtendedForGridAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OrderModel>> Post([FromBody] OrderModel model)
        {
            var result = await _orderService.CreateOrderAsync(model);
            return CreatedAtAction(nameof(Get), new { uid = result.f_uid }, result);
        }

        [HttpPut]
        public async Task<ActionResult<OrderModel>> Put([FromBody] OrderModel model)
        {
            var result = await _orderService.UpdateOrderAsync(model);
            return Ok(result);
        }

        [HttpDelete("{uid:guid}")]
        public async Task<ActionResult<int>> Delete(Guid uid)
        {
            var result = await _orderService.DeleteOrderAsync(uid);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
