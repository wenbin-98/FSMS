using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    public class QuotationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: QuotationController/Details/5
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

        // GET: QuotationController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuotationController/Create
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

        // GET: QuotationController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: QuotationController/Edit/5
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
