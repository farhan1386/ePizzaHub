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
        public async Task<ActionResult<IEnumerable<OrderModel>>> Get()
        {
            var result = await _orderService.GetOrdersAsync();
            return Ok(result);
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