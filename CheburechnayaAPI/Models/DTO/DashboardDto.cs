namespace CheburechnayaAPI.Models.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TodayOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalSuppliers { get; set; }
        public int ActiveDeliveries { get; set; }
        public int DeliveredOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class RecentOrderDto
    {
        public int Id { get; set; }
        public string Customer { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

}
