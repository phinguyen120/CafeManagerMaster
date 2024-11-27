using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace G11_Coffee.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CafeId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total Amount must be a positive value.")]
        public decimal TotalAmount { get; set; }

        // Order details fields
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
