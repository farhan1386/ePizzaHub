using ePizzaHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ePizzaHub.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PizzaApiClient");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Pizzas");
            }
            return View(new UserModel { f_email = "", f_password_hash = "", f_fname = "", f_lname = "", f_phone = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel model)
        {
            // Clear baseline validation parameters since registration fields aren't required here
            ModelState.Remove("f_fname");
            ModelState.Remove("f_lname");
            ModelState.Remove("f_phone");

            if (string.IsNullOrEmpty(model.f_email) || string.IsNullOrEmpty(model.f_password_hash))
            {
                ModelState.AddModelError("", "Email and Password are required fields.");
                return View(model);
            }

            // Post authentication request straight down to backend api endpoint route logic
            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/login", new
            {
                email = model.f_email,
                password = model.f_password_hash
            });

            if (response.IsSuccessStatusCode)
            {
                var verifiedUser = await response.Content.ReadFromJsonAsync<UserModel>();
                if (verifiedUser != null)
                {
                    // Create Frontend local Cookie Session Context based on returned backend profile metadata
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, verifiedUser.f_uid.ToString()),
                        new Claim(ClaimTypes.Email, verifiedUser.f_email),
                        new Claim(ClaimTypes.Name, $"{verifiedUser.f_fname} {verifiedUser.f_lname}"),
                        new Claim(ClaimTypes.MobilePhone, verifiedUser.f_phone)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Pizzas");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt. Please check your credentials.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Pizzas");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/register", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", "Registration failed. Email might already be taken.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Please provide your valid account email.");
                return View();
            }

            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/forgot-password", new { email = email });
            ViewBag.Message = "If that account exists in our master cluster, a reset link has been dispatched.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}