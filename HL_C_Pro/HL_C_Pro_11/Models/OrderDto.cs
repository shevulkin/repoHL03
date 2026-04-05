using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва товару є обов'язковою.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має містити від 3 до 100 символів.")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Кількість має бути від 1 до 1000.")]
        public int Quantity { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більшою за нуль.")]
        public decimal Price { get; set; }
    }
}