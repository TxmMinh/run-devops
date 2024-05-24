using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping.Client.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Shopping.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ShoppingAPIClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/Product");

            if (response == null)
            {
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);

            return View(products);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var response = await _httpClient.GetAsync($"/Product/{id}");

            if (response == null)
            {
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(content);

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.Id = GenerateHexId(24);
            // _logger.LogError("Ok");
            // if (!ModelState.IsValid)
            // {
            //     foreach (var state in ModelState)
            //     {
            //         foreach (var error in state.Value.Errors)
            //         {
            //             _logger.LogError("Property: {Property}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
            //         }
            //     }
            //     _logger.LogError("Invalid ModelState");
            //     return View(product);
            // }

            // Generate temporary ID for the product


            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Product", content);
            _logger.LogError("Test. Status code: {StatusCode}, Response: {Response}, Product : {Product}", response.StatusCode, await response.Content.ReadAsStringAsync(), json);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to add product. Status code: {StatusCode}, Response: {Response}, Product : {Product}", response.StatusCode, await response.Content.ReadAsStringAsync(), json);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"/Product/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            return RedirectToAction(nameof(Index));
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