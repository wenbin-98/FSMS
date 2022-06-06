namespace FSMS.Models
{
    public class StockDropdownViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        
        public StockDropdownViewModel(int id, string text, string desc, int quantity, double unitPrice)
        {
            this.id = id;
            this.text = text;
            this.description = desc;
            this.quantity = quantity;
            this.unitPrice = unitPrice;
        }
        
        public StockDropdownViewModel(int id, string text, string desc, double unitPrice)
        {
            this.id = id;
            this.text = text;
            this.description = desc;
            this.unitPrice = unitPrice;
        }
    }
}
