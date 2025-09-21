// Models/ViewModels/ProductFormViewModel.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FurnitureSalesSystem.Models.ViewModels
{
    public class ProductFormViewModel
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Display(Name = "Loại sản phẩm")]
        public int CategoryId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public IFormFile? ImageFile { get; set; }
    }
}
