using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.Web.Components
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public CartCountViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PizzaApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 1. Check for an active tracking browser session token
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                return View(0);
            }

            try
            {
                // 2. Fetch active records from your running backend API
                var cartItems = await _httpClient.GetFromJsonAsync<IEnumerable<CartItemModel>>($"api/CartItems?sessionUid={sessionUid}");
                if (cartItems != null)
                {
                    // 3. Sum up the quantities of all items currently in the basket
                    int totalItems = cartItems.Sum(item => item.f_quantity);
                    return View(totalItems);
                }
            }
            catch
            {
                // Safe structural fallback to empty state indicator on connection delays
                return View(0);
            }

            return View(0);
        }
    }
}