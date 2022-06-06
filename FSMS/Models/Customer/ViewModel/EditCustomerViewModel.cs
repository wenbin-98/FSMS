using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditCustomerViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Adress")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required]
        public string? Postcode { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        [Display(Name = "Contact No")]
        public string? Phone { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}
