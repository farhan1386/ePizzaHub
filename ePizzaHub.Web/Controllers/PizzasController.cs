using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ePizzaHub.Web.Controllers
{
    public class PizzasController : Controller
    {
        private readonly HttpClient _httpClient;

        public PizzasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PizzaApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                sessionUid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CustomerSessionUid", sessionUid);
            }

            var items = await _httpClient.GetFromJsonAsync<IEnumerable<PizzaModel>>("api/pizzas");
            return View(items ?? Array.Empty<PizzaModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid uid)
        {
            var match = await _httpClient.GetFromJsonAsync<PizzaModel>($"api/pizzas/{uid}");
            if (match == null)
            {
                return NotFound();
            }
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid pizzaUid, int quantity)
        {
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                sessionUid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CustomerSessionUid", sessionUid);
            }

            var cartItemPayload = new
            {
                f_customer_session_uid = sessionUid,
                f_pizza_uid = pizzaUid,
                f_quantity = quantity,
                f_create_date = DateTime.Now,
                f_create_by = Guid.Empty
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/CartItems", cartItemPayload);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "The API network engine declined basket data initialization.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to bridge session communication channels: {ex.Message}");
            }

            return RedirectToAction(nameof(Details), new { uid = pizzaUid });
        }

        [HttpPost]
        public async Task<IActionResult> GridData([FromBody] FilterModel filter)
        {
            var response = await _httpClient.PostAsJsonAsync("api/pizzas/grid", filter);
            if (response.IsSuccessStatusCode)
            {
                var gridResult = await response.Content.ReadFromJsonAsync<GridDataModel<PizzaModel>>();
                return Json(gridResult);
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PizzaModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/pizzas", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Server error creating pizza entity mapping profile.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid uid)
        {
            var match = await _httpClient.GetFromJsonAsync<PizzaModel>($"api/pizzas/{uid}");
            if (match == null)
            {
                return NotFound();
            }
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PizzaModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync("api/pizzas", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Server error modifying data properties context.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid uid)
        {
            var response = await _httpClient.DeleteAsync($"api/pizzas/{uid}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }
    }
}