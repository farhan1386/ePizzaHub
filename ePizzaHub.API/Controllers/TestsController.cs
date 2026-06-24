using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ePizzaHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestsController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                status = "Healthy",
                api_version = "v1",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
