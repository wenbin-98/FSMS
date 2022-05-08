using FSMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(int login)
        {
            if (login == 1)
            {
                ModelState.AddModelError("name", "Invalid Credentials");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(IFormCollection collection, int login)
        {

            string username = collection["Email"];
            string password = collection["Password"];
            if (username != "admin@gmail.com" || password != "12345")
            {
                ModelState.AddModelError("name", "Invalid Credentials");
                return View();
            }

            HttpContext.Response.Cookies.Append("username", "a",
                new CookieOptions { Expires = DateTime.Now.AddDays(30) });

            return RedirectToAction("Index", "Home");
        }

        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            ViewUserModel viewUserModel = new ViewUserModel();
            {
                viewUserModel.Username = "admin";
                viewUserModel.Name = "Manager";
                viewUserModel.Phone = "0167529609";
                viewUserModel.Address = "36, JLN KEBANGSAAN 22";
                viewUserModel.Address2 = "TMN UNIVERSITI";
                viewUserModel.Address3 = "81300 SKUDAI JOHOR";
            }
            return View(viewUserModel);
        }

        // GET: AccountController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountController/Create
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

        // GET: AccountController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
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

        // GET: AccountController/Delete/5
        public IActionResult Logout(int id)
        {
            HttpContext.Response.Cookies.Delete("username");
            return RedirectToAction("Login", "User");
        }
    }
}
