using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class AddDeliveryOrderViewModel : DocumentDetailViewModel
    {
        [Required]
        [Display(Name = "D/O No")]
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNo { get; set; }
        [Required]
        [Display(Name = "Purchase Order No")]
        public string? PurchaseOrder { get; set; }
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
    }
}
