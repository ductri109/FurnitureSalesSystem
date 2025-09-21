using System.ComponentModel.DataAnnotations;

namespace FurnitureSalesSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public int TotalOrders { get; set; } = 0;

        public bool IsVip { get; set; } = false;

        public ICollection<Order>? Orders { get; set; }
    }
}
