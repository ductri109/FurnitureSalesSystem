using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureSalesSystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Precision(18, 2)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Display(Name = "Loại sản phẩm")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(300)]
        public string? Image { get; set; }

        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // 🔥 Quan trọng: Cho phép upload ảnh từ view mà không lưu vào DB
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
