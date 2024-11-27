using System.ComponentModel.DataAnnotations;

namespace G11_Coffee.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
