using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditStockViewModel
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
