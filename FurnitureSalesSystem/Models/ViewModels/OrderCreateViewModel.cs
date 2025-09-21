using FurnitureSalesSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace FurnitureSalesSystem.Models.ViewModels
{
    public class OrderCreateViewModel
    {
        [Display(Name = "Khách hàng")]
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int CustomerId { get; set; }

        public int? OrderId { get; set; }

        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerPhone { get; set; } = "";

        public List<OrderItemViewModel> Items { get; set; } = new();

        [Display(Name = "Ghi chú nội bộ")]
        public string? InternalNote { get; set; }

        [Display(Name = "Ghi chú cho khách")]
        public string? CustomerNote { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

        public List<Customer> Customers { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
