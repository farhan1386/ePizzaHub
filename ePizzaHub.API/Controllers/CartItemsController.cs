using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly CartItemService _cartItemService;

        public CartItemsController(CartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemModel>>> Get()
        {
            var result = await _cartItemService.GetCartItemsAsync();
            return Ok(result);
        }

        [HttpGet("{uid:guid}")]
        public async Task<ActionResult<CartItemModel>> Get(Guid uid)
        {
            var result = await _cartItemService.GetCartItemByUidAsync(uid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridDataModel<CartItemModel>>> GetGrid([FromBody] FilterModel filter)
        {
            var result = await _cartItemService.GetCartItemsExtendedForGridAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CartItemModel>> Post([FromBody] CartItemModel model)
        {
            var result = await _cartItemService.CreateCartItemAsync(model);
            return CreatedAtAction(nameof(Get), new { uid = result.f_uid }, result);
        }

        [HttpPut]
        public async Task<ActionResult<CartItemModel>> Put([FromBody] CartItemModel model)
        {
            var result = await _cartItemService.UpdateCartItemAsync(model);
            return Ok(result);
        }

        [HttpDelete("{uid:guid}")]
        public async Task<ActionResult<int>> Delete(Guid uid)
        {
            var result = await _cartItemService.DeleteCartItemAsync(uid);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}