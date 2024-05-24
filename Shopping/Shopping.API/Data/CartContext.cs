using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Shopping.API.Models;

namespace Shopping.API.Data
{
    public class CartContext
    {
        public CartContext(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            Carts = database.GetCollection<Cart>(settings.Value.CartCollectionName);
            SeedData(Carts);
        }

        public IMongoCollection<Cart> Carts { get; }

        private static void SeedData(IMongoCollection<Cart> cartCollection)
        {
            bool existCart = cartCollection.Find(c => true).Any();
            if (!existCart)
            {
                cartCollection.InsertManyAsync(GetPreconfiguredCarts());
            }
        }

        private static IEnumerable<Cart> GetPreconfiguredCarts()
        {
            return new List<Cart>()
            {
                new Cart()
                {
                    Id = "1",
                    CartDate = DateTime.UtcNow,
                    CustomerName = "John Doe",
                    Items = new List<CartItem>()
                    {
                        new CartItem() { ProductName = "IPhone X", Price = 950.00M, Quantity = 1 },
                        new CartItem() { ProductName = "Samsung 10", Price = 840.00M, Quantity = 1 }
                    },
                    TotalPrice = 1790.00M
                }
            };
        }
    }
}
