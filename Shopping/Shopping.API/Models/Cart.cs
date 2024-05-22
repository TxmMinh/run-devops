using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Shopping.API.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public DateTime CartDate { get; set; }
        
        public string CustomerName { get; set; }
        
        public List<CartItem> Items { get; set; }
        
        public decimal TotalPrice { get; set; }
    }

    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal TotalPrice => Price * Quantity;
    }
}
