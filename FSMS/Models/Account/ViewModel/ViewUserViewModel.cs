using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class ViewUserViewModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        [Display(Name = "Contact No.")]
        public string? Phone { get; set; }
        public string Role { get; set; }
    }
}
