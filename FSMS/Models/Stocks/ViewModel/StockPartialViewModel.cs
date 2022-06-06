namespace FSMS.Models
{
    public class StockPartialViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class TestModel
    {
        public List<StockPartialViewModel> models { get; set; }
    }
}
