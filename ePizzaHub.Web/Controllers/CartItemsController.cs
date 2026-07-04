using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.Web.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartItemsController> _logger;

        public CartItemsController(IHttpClientFactory httpClientFactory, ILogger<CartItemsController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PizzaApiClient");
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                return View(Array.Empty<CartItemModel>());
            }

            try
            {
                var cartItemsTask = _httpClient.GetFromJsonAsync<IEnumerable<CartItemModel>>($"api/CartItems?sessionUid={sessionUid}");
                var pizzaCatalogTask = _httpClient.GetFromJsonAsync<IEnumerable<PizzaModel>>("api/Pizzas");

                await Task.WhenAll(cartItemsTask, pizzaCatalogTask);

                var cartItems = await cartItemsTask;
                var pizzaCatalog = await pizzaCatalogTask;

                if (cartItems != null && pizzaCatalog != null)
                {
                    var itemList = cartItems.ToList();

                    foreach (var item in itemList)
                    {
                        item.f_pizza = pizzaCatalog.FirstOrDefault(p => p.f_uid == item.f_pizza_uid);
                    }

                    decimal subTotal = itemList.Where(i => i.f_pizza != null)
                                              .Sum(i => i.f_pizza!.f_price * i.f_quantity);

                    ViewBag.SubTotal = subTotal;
                    ViewBag.TaxAmount = subTotal * 0.05m;
                    ViewBag.GrandTotal = subTotal + ViewBag.TaxAmount;

                    return View(itemList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync transaction state for session {SessionUid}", sessionUid);
                ModelState.AddModelError("", "Unable to load your shopping cart. Please try again later.");
            }

            return View(Array.Empty<CartItemModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Post(string f_pizza_uid, int f_quantity)
        {
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                sessionUid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CustomerSessionUid", sessionUid);
            }

            if (!Guid.TryParse(f_pizza_uid, out Guid pizzaGuid))
            {
                TempData["ErrorMessage"] = "Invalid pizza tracking specification package submitted.";
                return RedirectToAction("Index", "Pizzas");
            }

            try
            {
                var cartItemPayload = new CartItemModel
                {
                    f_customer_session_uid = sessionUid,
                    f_pizza_uid = pizzaGuid,
                    f_quantity = f_quantity
                };

                var response = await _httpClient.PostAsJsonAsync("api/CartItems", cartItemPayload);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Pizza added to your order successfully!";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("API backend refused to process cart entry request. Status Code: {StatusCode}", response.StatusCode);
                TempData["ErrorMessage"] = "Could not process your food item registration.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit transaction payload to backend API for session {SessionUid}", sessionUid);
                TempData["ErrorMessage"] = "A network connectivity exception occurred while updating your order status.";
            }

            return RedirectToAction("Index", "Pizzas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid uid)
        {
            if (uid == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Invalid item tracking identifier provided.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"api/CartItems/{uid}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Item successfully removed from your cart.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Failed to delete cart item {Uid}. API returned status code: {StatusCode}", uid, response.StatusCode);
                TempData["ErrorMessage"] = "The server could not process the removal request.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute deletion tracking handle for item {Uid}", uid);
                TempData["ErrorMessage"] = "A network error occurred while updating your cart.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}