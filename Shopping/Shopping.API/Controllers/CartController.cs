using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Shopping.API.Data;
using Shopping.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Shopping.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(CartContext context, ILogger<CartController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IEnumerable<Cart>> Get()
        {
            _logger.LogInformation("Fetching all carts");
            var carts = await _context.Carts.Find(c => true).ToListAsync();
            if (carts == null || !carts.Any())
            {
                _logger.LogWarning("No carts found in the database");
            }
            else
            {
                _logger.LogInformation($"{carts.Count} carts found in the database");
            }
            return carts;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cart cart)
        {
            if (cart == null)
            {
                _logger.LogError("Cart is null");
                return BadRequest("Cart is null");
            }

            await _context.Carts.InsertOneAsync(cart);
            _logger.LogInformation("Cart created successfully");
            return CreatedAtAction(nameof(Get), new { id = cart.Id }, cart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation($"Deleting cart with id: {id}");
            var result = await _context.Carts.DeleteOneAsync(c => c.Id == id);
            
            if (result.DeletedCount == 0)
            {
                _logger.LogWarning($"No cart found with id: {id}");
                return NotFound();
            }

            _logger.LogInformation($"Cart with id: {id} deleted successfully");
            return NoContent();
        }
    }
}
