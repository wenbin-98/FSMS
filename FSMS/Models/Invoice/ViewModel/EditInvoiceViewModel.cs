using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditInvoiceViewModel : DocumentDetailViewModel
    {
        [Required]
        [Display(Name = "Invoice No")]
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNo { get; set; }
        [Required]
        public List<InvoiceStocksDetails>? Stocks { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Validity")]
        public DateTime DueDate { get; set; }
        [Required]
        [Display(Name = "Purchase Order No")]
        public string? PurchaseOrder { get; set; }
    }
}
