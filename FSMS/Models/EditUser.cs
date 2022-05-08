using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Retype Password")]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
    }
}
