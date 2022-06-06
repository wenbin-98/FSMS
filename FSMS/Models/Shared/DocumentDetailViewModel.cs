using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public abstract class DocumentDetailViewModel
    {
        [Required(ErrorMessage = "Customer field is required")]
        [Display(Name = "Choose your customer")]
        public int CustomerId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public double Subtotal { get; set; }
        [Required]
        public double Tax { get; set; }
        [Required]
        public double ShippingFee { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
