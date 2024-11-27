//using G11_Coffee.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//public class RevenueController : Controller
//{
//    private readonly ApplicationDbContext _context;

//    public RevenueController(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? cafeId, int? employeeId)
//    {
//        if (User.Identity.IsAuthenticated)
//        {
//            var ordersQuery = _context.Orders
//            .Include(o => o.Employee)
//            .Include(o => o.Cafe)
//            .Include(o => o.OrderDetails)
//            .ThenInclude(od => od.Product)
//            .AsQueryable();

//            if (startDate.HasValue && endDate.HasValue)
//            {
//                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value && o.OrderDate <= endDate.Value);
//            }
//            if (cafeId.HasValue)
//            {
//                ordersQuery = ordersQuery.Where(o => o.CafeId == cafeId.Value);
//            }
//            if (employeeId.HasValue)
//            {
//                ordersQuery = ordersQuery.Where(o => o.EmployeeId == employeeId.Value);
//            }

//            var orders = await ordersQuery.ToListAsync();

//            var revenueViewModel = new RevenueViewModel
//            {
//                TotalRevenue = orders.Sum(o => o.TotalAmount),
//                Orders = orders ?? new List<Order>(),
//                StartDate = startDate,
//                EndDate = endDate,
//                CafeId = cafeId,
//                EmployeeId = employeeId,
//                Cafes = await _context.Cafes.ToListAsync() ?? new List<Cafe>(),
//                Employees = await _context.Employees.ToListAsync() ?? new List<Employee>(),
//                MonthlyRevenue = GetMonthlyRevenue(orders) ?? new Dictionary<string, decimal>(),
//                YearlyRevenue = GetYearlyRevenue(orders) ?? new Dictionary<string, decimal>(),
//                CafeRevenue = GetCafeRevenue(orders) ?? new Dictionary<string, decimal>(),
//                BestSellingProduct = GetBestSellingProduct(orders)
//            };

//            return View(revenueViewModel);
//        }
//        else
//        {
//            return RedirectToAction("Login", "Authentication");
//        }
//    }

//    //private Dictionary<string, decimal> GetMonthlyRevenue(List<Order> orders)
//    //{
//    //    return orders.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
//    //        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
//    //        .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => g.Sum(o => o.TotalAmount));
//    //}

//    //private Dictionary<string, decimal> GetYearlyRevenue(List<Order> orders)
//    //{
//    //    return orders.GroupBy(o => o.OrderDate.Year)
//    //        .OrderBy(g => g.Key)
//    //        .ToDictionary(g => g.Key.ToString(), g => g.Sum(o => o.TotalAmount));
//    //}

//    //private Dictionary<string, decimal> GetCafeRevenue(List<Order> orders)
//    //{
//    //    return orders.GroupBy(o => o.Cafe.Name)
//    //        .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));
//    //}

//    //private Product GetBestSellingProduct(List<Order> orders)
//    //{
//    //    if (!orders.Any() || !orders.SelectMany(o => o.OrderDetails).Any())
//    //    {
//    //        return null;
//    //    }

//    //    return orders.SelectMany(o => o.OrderDetails)
//    //        .GroupBy(od => od.Product)
//    //        .OrderByDescending(g => g.Sum(od => od.Quantity))
//    //        .First().Key;
//    //}
//}