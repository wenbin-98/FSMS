using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class StocksListViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double Price { get; set; }
    }
}
