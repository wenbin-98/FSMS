using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class InvoiceStocksDetails : DocumentStocksDetailsViewModel
    {
        [Required]
        public double UnitPrice { get; set; }
        //Only used for Details View
        public string? PictureUrl { get; set; }
    }
}
