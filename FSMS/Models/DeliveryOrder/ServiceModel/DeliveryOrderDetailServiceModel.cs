namespace FSMS.Models
{
    public class DeliveryOrderDetailServiceModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public List<DeliveryOrderStocksDetails>? Stocks { get; set; }
        public bool Status { get; set; }
        public string PurchaseOrder { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
    }
}
