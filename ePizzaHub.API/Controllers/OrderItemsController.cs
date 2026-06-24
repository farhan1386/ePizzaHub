using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly OrderItemService _orderItemService;

        public OrderItemsController(OrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemModel>>> Get()
        {
            var result = await _orderItemService.GetOrderItemsAsync();
            return Ok(result);
        }

        [HttpGet("{uid:guid}")]
        public async Task<ActionResult<OrderItemModel>> Get(Guid uid)
        {
            var result = await _orderItemService.GetOrderItemByUidAsync(uid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridDataModel<OrderItemModel>>> GetGrid([FromBody] FilterModel filter)
        {
            var result = await _orderItemService.GetOrderItemsExtendedForGridAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemModel>> Post([FromBody] OrderItemModel model)
        {
            var result = await _orderItemService.CreateOrderItemAsync(model);
            return CreatedAtAction(nameof(Get), new { uid = result.f_uid }, result);
        }

        [HttpPut]
        public async Task<ActionResult<OrderItemModel>> Put([FromBody] OrderItemModel model)
        {
            var result = await _orderItemService.UpdateOrderItemAsync(model);
            return Ok(result);
        }

        [HttpDelete("{uid:guid}")]
        public async Task<ActionResult<int>> Delete(Guid uid)
        {
            var result = await _orderItemService.DeleteOrderItemAsync(uid);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}