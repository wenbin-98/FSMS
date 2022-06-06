namespace FSMS.Models
{
    public class AddQuotationWrapper
    {
        public AddQuotationViewModel ViewModel { get; set; }
        public DocumentCustomerDetailViewModel Merchant { get; set; }
        public DocumentCustomerDetailViewModel Customer { get; set; }
        public AddProductViewModel AddProduct { get; set; }
        public QuotationRequestViewModel Quotation { get; set; }
    }
}
