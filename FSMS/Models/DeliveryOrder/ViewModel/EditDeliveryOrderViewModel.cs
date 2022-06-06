using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditDeliveryOrderViewModel
    {
        [Required]
        [Display(Name = "D/O No")]
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNo { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Purchase Order No")]
        public string? PurchaseOrder { get; set; }
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
    }
}
