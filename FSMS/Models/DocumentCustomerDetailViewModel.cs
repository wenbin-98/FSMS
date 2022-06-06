using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class DocumentCustomerDetailViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required]
        public string? Postcode { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        [Display(Name = "Contact No.")]
        public string? PhoneNo { get; set; }
        [Required]
        public string? Email { get; set; }

        public DocumentCustomerDetailViewModel(int id, string name, string address1, string address2, string postcode, string city, string state, string phoneNo, string email)
        {
            Id = id;
            Name = name;
            Address1 = address1;
            Address2 = address2;
            Postcode = postcode;
            City = city;
            State = state;
            PhoneNo = phoneNo;
            Email = email;
        }
    }
}
