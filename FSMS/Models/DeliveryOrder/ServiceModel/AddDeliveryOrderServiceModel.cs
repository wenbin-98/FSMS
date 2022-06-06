namespace FSMS.Models
{
    public class AddDeliveryOrderServiceModel
    {
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public string? PurchaseOrder { get; set; }
        public int CustomerId { get; set; }
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
    }
}
