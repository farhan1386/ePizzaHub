using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private readonly PizzaService _pizzaService;

        public PizzasController(PizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaModel>>> Get()
        {
            var result = await _pizzaService.GetPizzaAsync();
            return Ok(result);
        }

        [HttpGet("{uid:guid}")]
        public async Task<ActionResult<PizzaModel>> Get(Guid uid)
        {
            var result = await _pizzaService.GetPizzaByUidAsync(uid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridDataModel<PizzaModel>>> GetGrid([FromBody] FilterModel filter)
        {
            var result = await _pizzaService.GetPizzaExtendedForGridAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PizzaModel>> Post([FromBody] PizzaModel model)
        {
            var result = await _pizzaService.CreatePizzaAsync(model);
            return CreatedAtAction(nameof(Get), new { uid = result.f_uid }, result);
        }

        [HttpPut]
        public async Task<ActionResult<PizzaModel>> Put([FromBody] PizzaModel model)
        {
            var result = await _pizzaService.UpdatePizzaAsync(model);
            return Ok(result);
        }

        [HttpDelete("{uid:guid}")]
        public async Task<ActionResult<int>> Delete(Guid uid)
        {
            var result = await _pizzaService.DeletePizzaAsync(uid);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}