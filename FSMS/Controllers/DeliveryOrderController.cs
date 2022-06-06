using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FSMS.Controllers
{
    public class DeliveryOrderController : Controller
    {
        private readonly ILogger<DeliveryOrderController> _logger;
        private readonly IDeliveryOrderService _deliveryOrderService;
        private readonly ICustomerService _customerService;
        public DeliveryOrderController(ILogger<DeliveryOrderController> logger, IDeliveryOrderService deliveryOrderService, ICustomerService customerService)
        {
            _logger = logger;
            _deliveryOrderService = deliveryOrderService;
            _customerService = customerService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _deliveryOrderService.GetAllDeliveryOrderAsync();
            return View(viewModel);
        }

        // GET: DeliveryOrderController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _deliveryOrderService.GetDeliveryOrderDetail(id);

            ViewDeliveryOrderViewModel viewDeliveryOrderViewModel = new ViewDeliveryOrderViewModel()
            {
                Id = id,
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                Status = serviceModel.Status,
                PurchaseOrder = serviceModel.PurchaseOrder,
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
            };
            return View(viewDeliveryOrderViewModel);
        }

        // GET: DeliveryOrderController/Create
        public async Task<IActionResult> Create()
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            AddDeliveryOrderViewModel addDeliveryOrderViewModel = new AddDeliveryOrderViewModel()
            {
                Date = DateTime.Now,
                SerialNo = await _deliveryOrderService.GetLastestDeliveryOrderSerialNumberAsync()
            };

            AddDeliveryOrderWrapper addDeliveryOrderWrapper = new AddDeliveryOrderWrapper()
            {
                Merchant = documentMerchantDetail,
                ViewModel = addDeliveryOrderViewModel
            };
            return View(addDeliveryOrderWrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDeliveryOrder([FromBody] DeliveryOrderRequestViewModel viewModel)
        {
            try
            {
                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);

                if (!ModelState.IsValid) { return View(); }

                AddDeliveryOrderServiceModel addDeliveryOrderServiceModel = new AddDeliveryOrderServiceModel()
                {
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    PurchaseOrder = viewModel.PurchaseOrder,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _deliveryOrderService.CreateDeliveryOrderAsync(addDeliveryOrderServiceModel);

                TempData["Create"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch
            {
                return Json(HttpStatusCode.InternalServerError);
            }
        }

        // GET: DeliveryOrderController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _deliveryOrderService.GetDeliveryOrderDetail(id);
            EditDeliveryOrderViewModel editDeliveryOrderViewModel = new EditDeliveryOrderViewModel()
            {
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                Stocks = serviceModel.Stocks,
                PurchaseOrder = serviceModel.PurchaseOrder
            };

            EditDeliveryOrderWrapper editInvoiceWrapper = new EditDeliveryOrderWrapper
            {
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
                ViewModel = editDeliveryOrderViewModel
            };
            ViewBag.CustomerId = serviceModel.Customer.Id;
            ViewBag.Id = id;
            return View(editInvoiceWrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeliveryOrder([FromBody] DeliveryOrderRequestViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }
                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);

                EditDeliveryOrderServiceModel editInvoiceServiceModel = new EditDeliveryOrderServiceModel()
                {
                    Id = viewModel.Id,
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    PurchaseOrder = viewModel.PurchaseOrder,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _deliveryOrderService.UpdateDeliveryOrderAsync(editInvoiceServiceModel);
                TempData["Edit"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Json(HttpStatusCode.InternalServerError);
            }
        }

        // POST: StocksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                if (collection.Keys.Contains("item.Id"))
                {
                    await _deliveryOrderService.DeleteDeliveryOrderAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _deliveryOrderService.DeleteDeliveryOrderAsync(int.Parse(collection["Id"]));
                }

                TempData["Delete"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(IFormCollection collection)
        {
            try
            {
                await _deliveryOrderService.ChangeDeliveryOrderStatusAsync(int.Parse(collection["Id"]));
                return RedirectToAction("Details", new { id = collection["Id"] });
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
