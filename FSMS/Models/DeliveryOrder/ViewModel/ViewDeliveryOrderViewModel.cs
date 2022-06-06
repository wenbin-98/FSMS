namespace FSMS.Models
{
    public class ViewDeliveryOrderViewModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<QuotationStocksDetails>? Stocks { get; set; }
        public double Subtotal { get; set; }
        public double Tax { get; set; }
        public double ShippingFee { get; set; }
        public double Price { get; set; }
        public string PurchaseOrder { get; set; }
        public bool Status { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
    }
}
