namespace FSMS.Models
{
    public class EditInvoiceServiceModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public double Subtotal { get; set; }
        public double Tax { get; set; }
        public double ShippingFee { get; set; }
        public double Price { get; set; }
        public string? PurchaseOrder { get; set; }
        public int CustomerId { get; set; }
        public List<InvoiceStocksDetails>? Stocks { get; set; }
    }
}
