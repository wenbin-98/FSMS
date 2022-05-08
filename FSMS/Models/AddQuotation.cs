namespace FSMS.Models
{
    public class AddQuotation
    {
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public int MyProperty { get; set; }
        public string DueDate { get; set; }

        public List<Stocks> Stocks { get; set; }
        public string Price { get; set; }
    }
}
