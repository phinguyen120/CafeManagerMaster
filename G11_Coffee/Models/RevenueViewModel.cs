using G11_Coffee.Models;

namespace G11_Coffee.Models
{
    public class RevenueViewModel
    {
        public decimal TotalRevenue { get; set; }
      //  public List<Order> Orders { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CafeId { get; set; }
        public int? EmployeeId { get; set; }
        public List<Cafe> Cafes { get; set; }
        //public List<Employee> Employees { get; set; }
        public Dictionary<string, decimal> MonthlyRevenue { get; set; }
        public Dictionary<string, decimal> YearlyRevenue { get; set; }
        public Dictionary<string, decimal> CafeRevenue { get; set; }
        public Product? BestSellingProduct { get; set; }
    }
}