namespace FSMS.Models
{
    public class EditInvoiceWrapper
    {
        public EditInvoiceViewModel ViewModel { get; set; }
        public DocumentCustomerDetailViewModel Merchant { get; set; }
        public DocumentCustomerDetailViewModel Customer { get; set; }
        public AddProductViewModel AddProduct { get; set; }
        public InvoiceRequestViewModel Invoice { get; set; }
    }
}
