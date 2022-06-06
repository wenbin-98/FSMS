namespace FSMS.Models
{
    public class EditDeliveryOrderServiceModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public string? PurchaseOrder { get; set; }
        public int CustomerId { get; set; }
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
    }
}
