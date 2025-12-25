using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Получаем общую статистику
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
            var deliveredOrders = await _context.Orders.CountAsync(o => o.Status == "Delivered");

            var todayOrders = await _context.Orders
                .CountAsync(o => o.OrderDate.Date == today);

            var monthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth)
                .SumAsync(o => o.TotalAmount);

            var totalProducts = await _context.Products.CountAsync();

            // Низкий запас - менее 20 штук
            var lowStockProducts = await _context.Products
                .Where(p => p.OrderItems.Sum(oi => oi.Quantity) < 20)
                .CountAsync();

            var totalEmployees = await _context.Employees.CountAsync();
            var totalSuppliers = await _context.Suppliers.CountAsync();

            var activeDeliveries = await _context.Deliveries
                .CountAsync(d => d.Status == "Pending" || d.Status == "In Transit");

            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var stats = new DashboardStatsDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                TotalProducts = totalProducts,
                LowStockProducts = lowStockProducts,
                TodayOrders = todayOrders,
                MonthlyRevenue = monthlyRevenue,
                TotalEmployees = totalEmployees,
                TotalSuppliers = totalSuppliers,
                ActiveDeliveries = activeDeliveries,
                DeliveredOrders = deliveredOrders,
                AverageOrderValue = Math.Round(averageOrderValue, 2)
            };

            return Ok(stats);
        }

        // GET: api/dashboard/recent-orders
        [HttpGet("recent-orders")]
        public async Task<ActionResult<IEnumerable<RecentOrderDto>>> GetRecentOrders([FromQuery] int limit = 5)
        {
            var recentOrders = await _context.Orders
                .Include(o => o.Employee)
                .OrderByDescending(o => o.OrderDate)
                .Take(limit)
                .Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    Customer = o.Employee.FullName,
                    Amount = o.TotalAmount,
                    Status = o.Status,
                    Time = o.OrderDate.ToString("HH:mm")
                })
                .ToListAsync();

            return Ok(recentOrders);
        }

        // GET: api/dashboard/popular-products
        [HttpGet("popular-products")]
        public async Task<ActionResult<IEnumerable<PopularProductDto>>> GetPopularProducts([FromQuery] int limit = 5)
        {
            var popularProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(oi => oi.Product)
                .Select(g => new PopularProductDto
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.ProductName,
                    Category = g.Key.Category
                })
                .Take(limit)
                .ToListAsync();

            return Ok(popularProducts);
        }

        // GET: api/dashboard/employee-performance
        [HttpGet("employee-performance")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeePerformance()
        {
            var performance = await _context.Employees
                .Include(e => e.Orders)
                .Select(e => new
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    Position = e.Position.Title,
                    TotalOrders = e.Orders.Count,
                    TotalRevenue = e.Orders.Sum(o => o.TotalAmount),
                    AverageOrderValue = e.Orders.Count > 0 ?
                        e.Orders.Average(o => o.TotalAmount) : 0
                })
                .OrderByDescending(e => e.TotalRevenue)
                .ToListAsync();

            return Ok(performance);
        }

        // GET: api/dashboard/revenue-by-category
        [HttpGet("revenue-by-category")]
        public async Task<ActionResult<IEnumerable<object>>> GetRevenueByCategory()
        {
            var revenueByCategory = await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.Product.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    TotalItems = g.Sum(oi => oi.Quantity),
                    ProductCount = g.Select(oi => oi.ProductId).Distinct().Count()
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToListAsync();

            return Ok(revenueByCategory);
        }

        // GET: api/dashboard/monthly-stats
        [HttpGet("monthly-stats")]
        public async Task<ActionResult<IEnumerable<object>>> GetMonthlyStats()
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var monthlyStats = await _context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new { Year = o.OrderDate.Year, Month = o.OrderDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Month:00}/{g.Key.Year}",
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(m => m.Period)
                .ToListAsync();

            return Ok(monthlyStats);
        }
    }
}