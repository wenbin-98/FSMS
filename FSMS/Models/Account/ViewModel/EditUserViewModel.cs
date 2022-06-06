using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditUserViewModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        [DataType(DataType.Password)]
        public string? RetypePassword { get; set; }
        [Required]
        [Display(Name = "Contact No.")]
        public string? Phone { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
