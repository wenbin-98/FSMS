using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class ViewInvoiceViewModel
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:00000}", ApplyFormatInEditMode = true)]
        public int SerialNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<InvoiceStocksDetails>? Stocks { get; set; }
        public double Subtotal { get; set; }
        public double Tax { get; set; }
        public double ShippingFee { get; set; }
        public double Price { get; set; }
        public bool PaymentStatus { get; set; }
        public string? PurchaseOrder { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
    }
}
