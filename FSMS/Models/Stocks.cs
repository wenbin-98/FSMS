using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class Stocks
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
