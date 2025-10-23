using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureSalesSystem.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public string UserId { get; set; } // User từ IdentityUser
        public IdentityUser? User { get; set; }
        [MaxLength(255)]
        public string? InternalNote { get; set; }

        [MaxLength(255)]
        public string? CustomerNote { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public string? CancelReason { get; set; }     // Lý do chính
        public DateTime? CancelledAt { get; set; }    // Thời điểm hủy
        public string? CancelledBy { get; set; }      // Người hủy


    }
}
