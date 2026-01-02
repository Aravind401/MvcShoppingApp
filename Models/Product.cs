using System.ComponentModel.DataAnnotations;

namespace MvcShoppingApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, 10000)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;
        
        [Display(Name = "Image")]
        public string ImageUrl { get; set; } = "/images/default.jpg";
        
        [Display(Name = "In Stock")]
        public int StockQuantity { get; set; } = 10;
        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}


//Admin: admin @shop.com / admin123

//Customer: customer @shop.com / customer123