using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class UserList
    {
        public string Username { get; set; }
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }
    }
}
