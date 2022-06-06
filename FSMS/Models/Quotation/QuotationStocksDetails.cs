namespace FSMS.Models
{
    public class QuotationStocksDetails : DocumentStocksDetailsViewModel
    {
        public double UnitPrice { get; set; }
        //Only used for Details View
        public string? PictureUrl { get; set; }
    }
}
