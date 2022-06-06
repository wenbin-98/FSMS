namespace FSMS.Models
{
    public class AddInvoiceWrapper
    {
        public AddInvoiceViewModel ViewModel { get; set; }
        public DocumentCustomerDetailViewModel Merchant { get; set; }
        public DocumentCustomerDetailViewModel Customer { get; set; }
        public AddProductViewModel AddProduct { get; set; }
        public InvoiceRequestViewModel Invoice { get; set; }
    }
}
