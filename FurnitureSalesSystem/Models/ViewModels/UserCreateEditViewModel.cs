using System.ComponentModel.DataAnnotations;

namespace FurnitureSalesSystem.Models.ViewModels
{
    public class UserCreateEditViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? Password { get; set; }  // ✅ KHÔNG [Required]

        public List<string>? Roles { get; set; } = new List<string>();
    }


}
