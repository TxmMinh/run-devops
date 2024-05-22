using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping.Client.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Shopping.Client.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ShoppingAPIClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/Cart");
            if (response == null || !response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch carts. Response: {Response}", response);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var content = await response.Content.ReadAsStringAsync();
            var carts = JsonConvert.DeserializeObject<List<Cart>>(content);

            return View(carts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Product product, int quantity)
        {
            try
            {
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };

                var cart = new Cart
                {
                    Id = GenerateHexId(24),
                    CustomerName = "DefaultCustomer",
                    CartDate = DateTime.Now,
                    Items = new List<CartItem> { cartItem },
                    TotalPrice = cartItem.TotalPrice
                };

                var jsonContent = JsonConvert.SerializeObject(cart);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Cart", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to add to cart. Status code: {StatusCode}, Response: {Response}, Cart : {Cart}", response.StatusCode, await response.Content.ReadAsStringAsync(), jsonContent);
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding to cart");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        private static string GenerateHexId(int length)
        {
            var buffer = new byte[length / 2];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return string.Concat(buffer.Select(b => b.ToString("x2")));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
