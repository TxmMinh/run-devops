using System.ComponentModel.DataAnnotations;

namespace Shopping.Client.Models
{
    public class Product
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ImageFile is required")]
        public string ImageFile { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 1000000")]
        public decimal Price { get; set; }
    }
}
