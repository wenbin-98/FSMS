using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public abstract class DocumentRequestModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Customer")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer field is required.")]
        public int CustomerId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}
