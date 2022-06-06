using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public abstract class DocumentStocksDetailsViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
