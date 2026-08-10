using peejayworld_mvc.Models;

namespace peejayworld_mvc.ViewModels;

public class AdminDashboardViewModel
{
    public IEnumerable<Order> RecentOrders { get; set; } = Enumerable.Empty<Order>();
    public decimal TotalSalesLast7Days { get; set; }
    public decimal TotalSalesLast30Days { get; set; }
    public decimal TotalSalesLast365Days { get; set; }
    public IEnumerable<string> ChartLabels { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<decimal> ChartValues { get; set; } = Enumerable.Empty<decimal>();
}
