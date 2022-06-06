namespace FSMS.Models
{
    public class EditDeliveryOrderWrapper
    {
        public EditDeliveryOrderViewModel ViewModel { get; set; }
        public DocumentCustomerDetailViewModel? Merchant { get; set; }
        public DocumentCustomerDetailViewModel? Customer { get; set; }
        public DeliveryOrderRequestViewModel DeliveryOrder { get; set; }
        public AddProductViewModel AddProduct { get; set; }
    }
}
