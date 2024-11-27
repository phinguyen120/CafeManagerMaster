using System.ComponentModel.DataAnnotations;

namespace G11_Coffee.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public int CafeId { get; set; }

        public Cafe Cafe { get; set; }
    }
}
