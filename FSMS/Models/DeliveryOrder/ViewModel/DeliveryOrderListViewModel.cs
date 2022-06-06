using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class DeliveryOrderListViewModel
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNumber { get; set; }
        public string? CustomerName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
