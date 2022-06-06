using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class AddQuotationViewModel : DocumentDetailViewModel
    {
        [Required]
        [Display(Name = "Quotation No")]
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNo { get; set; }
        [Required]
        public List<QuotationStocksDetails>? Stocks { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Validity")]
        public DateTime DueDate { get; set; }
    }
}
