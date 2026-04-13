
using System.ComponentModel.DataAnnotations;

namespace Common.Dtos
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public string Colour { get; set; } = string.Empty;
        [Required]
        public string? ImageUrl { get; set; }
    }
}
