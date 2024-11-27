using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace G11_Coffee.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
        public string Image { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }

        // Foreign key to Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int CafeId { get; set; }
        public Cafe Cafe { get; set; }
    }
}
