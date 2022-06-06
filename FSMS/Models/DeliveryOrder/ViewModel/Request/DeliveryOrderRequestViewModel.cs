using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class DeliveryOrderRequestViewModel : DocumentRequestModel
    {
        [Required]
        [Display(Name = "D/O #")]
        public int SerialNo { get; set; }
        [Required(ErrorMessage = "Product cannot be empty")]
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
        [Required]
        [Display(Name = "Purchase Order No")]
        public string? PurchaseOrder { get; set; }
    }
}
