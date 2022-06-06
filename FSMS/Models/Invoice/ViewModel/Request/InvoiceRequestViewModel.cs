using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class InvoiceRequestViewModel : DocumentRequestModel
    {
        [Required]
        [Display(Name = "Quotation #")]
        public int SerialNo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Validity")]
        public DateTime DueDate { get; set; }
        [Required]
        public double Subtotal { get; set; }
        [Required]
        public double Tax { get; set; }
        [Display(Name = "Shipping Fee")]
        public double ShippingFee { get; set; }
        [Required]
        [Display(Name = "Total")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Purchase Order No")]
        public string? PurchaseOrder { get; set; }
        [Required(ErrorMessage = "Product cannot be empty")]
        public List<InvoiceStocksDetails>? Stocks { get; set; }
    }
}
