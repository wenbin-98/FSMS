using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: InvoiceController/Details/5
        public IActionResult Details(int id)
        {
            if (id == 1)
            {
                TempData["Create"] = true;

            }
            if (id == 2)
            {
                TempData["Edit"] = true;

            }
            return View();
        }

        // GET: InvoiceController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InvoiceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvoiceController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
