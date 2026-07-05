using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PizzaApiClient");
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }

            try
            {
                OrderModel? order = null;

                if (Guid.TryParse(id, out Guid exactGuid))
                {
                    order = await _httpClient.GetFromJsonAsync<OrderModel>($"api/Orders/{exactGuid}");
                }
                else
                {
                    order = await _httpClient.GetFromJsonAsync<OrderModel>($"api/Orders/search/{id}");
                }

                if (order != null)
                {
                    return View(order);
                }

                TempData["ErrorMessage"] = "No active pizza order details could be located with that tracking reference number.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pull tracking state matrices for sequence profile: {OrderId}", id);
                TempData["ErrorMessage"] = "No matching order records were found inside our database.";
            }

            return View();
        }
    }
}