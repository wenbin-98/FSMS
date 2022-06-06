namespace FSMS.Models
{
    public class CustomerDropdownViewModel
    {
        public int id { get; set; }
        public string text { get; set; }

        public CustomerDropdownViewModel(int id, string text)
        {
            this.id = id;
            this.text = text;
        }
    }
}
