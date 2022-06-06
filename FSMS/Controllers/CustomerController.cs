using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FSMS.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }
        // GET: CustomerController
        public async Task<IActionResult> Index()
        {
            var viewModel = await _customerService.GetAllCustomerAsync();
            return View(viewModel);
        }

        // GET: CustomerController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var serviceModel = await _customerService.GetCustomerDetailsAsync(id);
            ViewCustomerViewModel viewModel = new ViewCustomerViewModel
            {
                Id = id,
                Name = serviceModel.Name,
                Address1 = serviceModel.Address1,
                Address2 = serviceModel.Address2,
                Postcode = serviceModel.Postcode,
                City = serviceModel.City,
                State = serviceModel.State,
                Phone = serviceModel.Phone,
                Email = serviceModel.Email
            };
            return View(viewModel);
        }

        // GET: CustomerController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                AddCustomerServiceModel addCustomerServiceModel = new AddCustomerServiceModel()
                {
                    Name = collection["Name"],
                    Address1 = collection["Address1"],
                    Address2 = collection["Address2"],
                    Postcode = collection["Postcode"],
                    City = collection["City"],
                    State = collection["State"],
                    Phone = collection["Phone"],
                    Email = collection["Email"],
                };

                await _customerService.CreateCustomerAsync(addCustomerServiceModel);

                TempData["Create"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: CustomerController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await _customerService.GetCustomerDetailsAsync(id);
            EditCustomerViewModel viewModel = new EditCustomerViewModel
            {
                Name = serviceModel.Name,
                Address1 = serviceModel.Address1,
                Address2 = serviceModel.Address2,
                Postcode = serviceModel.Postcode,
                City = serviceModel.City,
                State = serviceModel.State,
                Phone = serviceModel.Phone,
                Email = serviceModel.Email
            };
            return View(viewModel);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                EditCustomerServiceModel editCustomerServiceModel = new EditCustomerServiceModel()
                {
                    Id = id,
                    Name = collection["Name"],
                    Address1 = collection["Address1"],
                    Address2 = collection["Address2"],
                    Postcode = collection["Postcode"],
                    City = collection["City"],
                    State = collection["State"],
                    Phone = collection["Phone"],
                    Email = collection["Email"],
                };

                await _customerService.EditCustomerAsync(editCustomerServiceModel);

                TempData["Create"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                if (collection.Keys.Contains("item.Id"))
                {
                    await _customerService.DeleteCustomerAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _customerService.DeleteCustomerAsync(int.Parse(collection["Id"]));
                }

                TempData["Delete"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> RenderCustomerDetailPartialView(int id)
        {
            var serviceModel = await _customerService.GetCustomerDetailsAsync(id);
            EditCustomerViewModel viewModel = new EditCustomerViewModel
            {
                Name = serviceModel.Name,
                Address1 = serviceModel.Address1,
                Address2 = serviceModel.Address2,
                Postcode = serviceModel.Postcode,
                City = serviceModel.City,
                State = serviceModel.State,
                Phone = serviceModel.Phone,
                Email = serviceModel.Email
            };
            DocumentCustomerDetailViewModel documentCustomerDetail = new DocumentCustomerDetailViewModel(serviceModel.Id, serviceModel.Name, serviceModel.Address1, serviceModel.Address2, serviceModel.Postcode, serviceModel.City, serviceModel.State, serviceModel.Phone, serviceModel.Email);
            return PartialView("_CustomerDetailPartiaView", documentCustomerDetail);
        }

        public async Task<IActionResult> GetCustomerDropDownData(string q, int id)
        {
            List<CustomerDetailServiceModel> serviceModel = await _customerService.GetCustomerDropDownAsync(q);
            List<CustomerDropdownViewModel> customerDropdowns = new List<CustomerDropdownViewModel>();

            if (serviceModel.Count > 0)
            {
                for (var i = 0; i < serviceModel.Count; i++)
                {
                    customerDropdowns.Add(new CustomerDropdownViewModel(serviceModel[i].Id, serviceModel[i].Name));
                }
            }

            return Json(customerDropdowns);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomerDetail(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                EditCustomerServiceModel editCustomerServiceModel = new EditCustomerServiceModel()
                {
                    Id = int.Parse(collection["Merchant.Id"]),
                    Name = collection["Merchant.Name"],
                    Address1 = collection["Merchant.Address1"],
                    Address2 = collection["Merchant.Address2"],
                    Postcode = collection["Merchant.Postcode"],
                    City = collection["Merchant.City"],
                    State = collection["Merchant.State"],
                    Phone = collection["Merchant.Phone"],
                    Email = collection["Merchant.Email"],
                };

                await _customerService.EditCustomerAsync(editCustomerServiceModel);

                TempData["Create"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch
            {
                return Json(HttpStatusCode.BadRequest);
            }
        }
    }
}
