using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    public class SalesReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: SalesReportController/Details/5
        public FileResult Download(int id)
        {
            string fileName = "sales report.pdf";
            //Build the File Path.
            string path = Path.Combine(Environment.CurrentDirectory, "wwwroot/", "Files/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        // GET: SalesReportController/Details/5
        public FileResult GetReport(int id)
        {
            string fileName = "sales report.pdf";
            //Build the File Path.
            string path = Path.Combine(Environment.CurrentDirectory, "wwwroot/", "Files/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/pdf");
        }
    }
}
