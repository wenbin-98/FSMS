using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    public class StocksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: StocksController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: StocksController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StocksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                TempData["Create"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: StocksController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: StocksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                TempData["Edit"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
