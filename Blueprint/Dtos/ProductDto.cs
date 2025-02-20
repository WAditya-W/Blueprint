using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Blueprint.Dtos
{
    [ExcludeFromCodeCoverage]
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, 999999999.99, ErrorMessage = "Price must be between 0.01 and 999999999.99")]
        public decimal Price { get; set; }
    }
}
