namespace FSMS.Models
{
    public class StockDetalsServiceModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Picture { get; set; }
    }
}
