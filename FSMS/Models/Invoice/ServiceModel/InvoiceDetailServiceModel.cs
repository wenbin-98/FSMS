namespace FSMS.Models
{
    public class InvoiceDetailServiceModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<InvoiceStocksDetails>? Stocks { get; set; }
        public double Subtotal { get; set; }
        public double Tax { get; set; }
        public double ShippingFee { get; set; }
        public double Price { get; set; }
        public bool PaymentStatus { get; set; }
        public string PurchaseOrder { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
    }
}
