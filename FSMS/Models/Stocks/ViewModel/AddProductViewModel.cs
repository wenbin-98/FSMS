using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class AddProductViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Quantity { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Unit Price (RM)")]
        public double UnitPrice { get; set; }

        public AddProductViewModel(int id, int quantity, string name, string desc, double unitPrice)
        {
            Id = id;
            Quantity = quantity;
            Name = name;
            Description = desc;
            UnitPrice = unitPrice;
        }
    }
}
