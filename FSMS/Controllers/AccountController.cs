using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FSMS.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _accountService.GetAllUserAsync();
            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Login(int login)
        {
            if (login == 1)
            {
                ModelState.AddModelError("name", "Invalid Credentials");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                string username = collection["Username"];
                string password = collection["Password"];
                bool IsPersistent = bool.Parse(collection["RememberMe"]);

                SignInServiceModel signInServiceModel = await _accountService.LoginAccountAsync(username, password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, signInServiceModel.Name),
                    new Claim(ClaimTypes.Role, signInServiceModel.Role),
                    new Claim(ClaimTypes.NameIdentifier, signInServiceModel.Username)
                };

                var identity = new ClaimsIdentity(claims, "MyAuthCookie");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = IsPersistent,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsPrincipal),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: AccountController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var serviceModel = await _accountService.GetUserDetailsAsync(id);
            ViewUserViewModel viewUserModel = new ViewUserViewModel();
            {
                viewUserModel.Username = serviceModel.Username;
                viewUserModel.Name = serviceModel.Name;
                viewUserModel.Phone = serviceModel.PhoneNo;
                viewUserModel.Role = serviceModel.Role;
                viewUserModel.Id = serviceModel.Id;
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
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                AddUserServiceModel addUserServiceModel = new AddUserServiceModel()
                {
                    Name = collection["Name"],
                    Username = collection["Username"],
                    Password = collection["Password"],
                    Role = collection["Role"],
                    PhoneNo = collection["Phone"]
                };
                bool result = await _accountService.CreateUserAsync(addUserServiceModel);
                
                if (!result)
                {
                    throw new Exception("Internal Server Error!");
                }
                TempData["Create"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: AccountController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var serviceModel = await _accountService.GetUserDetailsAsync(id);
                EditUserViewModel editUserViewModel = new EditUserViewModel()
                {
                    Name = serviceModel.Name,
                    Username = serviceModel.Username,
                    Phone = serviceModel.PhoneNo,
                    Role = serviceModel.Role
                };
                return View(editUserViewModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                EditUserServiceModel editUserServiceModel = new EditUserServiceModel()
                {
                    Id = id,
                    Name = collection["Name"],
                    Username = collection["Username"],
                    Password = collection["Password"],
                    Role = collection["Role"],
                    PhoneNo = collection["Phone"]
                };
                bool result = await _accountService.EditUserAsync(editUserServiceModel);

                if (!result)
                {
                    throw new Exception("Internal Server Error!");
                }
                TempData["Edit"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                if (collection.Keys.Contains("item.Id"))
                {
                    await _accountService.DeleteUserAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _accountService.DeleteUserAsync(int.Parse(collection["Id"]));
                }

                TempData["Delete"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public async Task<IActionResult> LogoutAsync(int id)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
