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
            string? sessionUid = HttpContext.Session.GetString("CustomerSessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                return View(0);
            }

            try
            {
                var cartItems = await _httpClient.GetFromJsonAsync<IEnumerable<CartItemModel>>($"api/CartItems?sessionUid={sessionUid}");
                if (cartItems != null)
                {
                    int totalItems = cartItems.Sum(item => item.f_quantity);
                    return View(totalItems);
                }
            }
            catch
            {
                return View(0);
            }

            return View(0);
        }
    }
}