using System.ComponentModel.DataAnnotations;

namespace FSMS.Models
{
    public class EditQuotationWrapper
    {
        public EditQuotationViewModel ViewModel { get; set; }
        public DocumentCustomerDetailViewModel Merchant { get; set; }
        public DocumentCustomerDetailViewModel Customer { get; set; }
        public AddProductViewModel AddProduct { get; set; }
        public QuotationRequestViewModel Quotation { get; set; }
    }
}
