using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FSMS.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceService invoiceService, ICustomerService customerService)
        {
            _logger = logger;
            _invoiceService = invoiceService;
            _customerService = customerService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _invoiceService.GetAllInvoiceAsync();
            return View(viewModel);
        }

        // GET: InvoiceController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _invoiceService.GetInvoiceDetail(id);

            ViewInvoiceViewModel viewInvoiceViewModel = new ViewInvoiceViewModel()
            {
                Id = id,
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                DueDate= serviceModel.DueDate,
                Subtotal = serviceModel.Subtotal,
                Tax = serviceModel.Tax,
                ShippingFee = serviceModel.ShippingFee,
                Price = serviceModel.Price,
                PaymentStatus = serviceModel.PaymentStatus,
                PurchaseOrder = serviceModel.PurchaseOrder,
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
                Stocks = serviceModel?.Stocks,
            };
            return View(viewInvoiceViewModel);
        }

        // GET: InvoiceController/Create
        public async Task<IActionResult> Create()
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            AddInvoiceViewModel addInvoiceViewModel = new AddInvoiceViewModel()
            {
                Date = DateTime.Now,
                DueDate = DateTime.Now,
                SerialNo = await _invoiceService.GetLastestInvoiceSerialNumberAsync()
            };

            AddInvoiceWrapper addInvoiceWrapper = new AddInvoiceWrapper()
            {
                Merchant = documentMerchantDetail,
                ViewModel = addInvoiceViewModel
            };
            return View(addInvoiceWrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoice([FromBody] InvoiceRequestViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);
                viewModel.DueDate = TimeZoneInfo.ConvertTimeFromUtc(viewModel.DueDate, TimeZoneInfo.Local);

                AddInvoiceServiceModel addInvoiceServiceModel = new AddInvoiceServiceModel()
                {
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    DueDate = viewModel.DueDate,
                    Subtotal = viewModel.Subtotal,
                    Tax = viewModel.Tax,
                    ShippingFee = viewModel.ShippingFee,
                    Price = viewModel.Price,
                    PurchaseOrder = viewModel.PurchaseOrder,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _invoiceService.CreateInvoiceAsync(addInvoiceServiceModel);

                TempData["Create"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Json(HttpStatusCode.InternalServerError);
            }
        }

        // GET: InvoiceController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _invoiceService.GetInvoiceDetail(id);
            EditInvoiceViewModel editInvoiceViewModel = new EditInvoiceViewModel()
            {
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                DueDate = serviceModel.DueDate,
                Subtotal = serviceModel.Subtotal,
                Tax = serviceModel.Tax,
                ShippingFee = serviceModel.ShippingFee,
                Price = serviceModel.Price,
                Stocks = serviceModel.Stocks,
                PurchaseOrder = serviceModel.PurchaseOrder
            };

            EditInvoiceWrapper editInvoiceWrapper = new EditInvoiceWrapper
            {
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
                ViewModel = editInvoiceViewModel
            };
            ViewBag.CustomerId = serviceModel.Customer.Id;
            ViewBag.Id = id;
            return View(editInvoiceWrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInvoice([FromBody] InvoiceRequestViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);
                viewModel.DueDate = TimeZoneInfo.ConvertTimeFromUtc(viewModel.DueDate, TimeZoneInfo.Local);

                EditInvoiceServiceModel editInvoiceServiceModel = new EditInvoiceServiceModel()
                {
                    Id = viewModel.Id,
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    DueDate = viewModel.DueDate,
                    Subtotal = viewModel.Subtotal,
                    Tax = viewModel.Tax,
                    ShippingFee = viewModel.ShippingFee,
                    Price = viewModel.Price,
                    PurchaseOrder = viewModel.PurchaseOrder,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _invoiceService.UpdateInvoiceAsync(editInvoiceServiceModel);
                TempData["Edit"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch(Exception ex)
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
                    await _invoiceService.DeleteInvoiceAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _invoiceService.DeleteInvoiceAsync(int.Parse(collection["Id"]));
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
        public async Task<IActionResult> ChangeQuotationStatus(IFormCollection collection)
        {
            try
            {
                await _invoiceService.ChangePaymentStatusAsync(int.Parse(collection["Id"]));
                return RedirectToAction("Details", new { id = collection["Id"] });
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
