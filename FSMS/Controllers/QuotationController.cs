using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FSMS.Controllers
{
    public class QuotationController : Controller
    {
        private readonly ILogger<QuotationController> _logger;
        private readonly IQuotationService _quotationService;
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        public QuotationController(ILogger<QuotationController> logger, IQuotationService quotationService, ICustomerService customerService, IInvoiceService invoiceService)
        {
            _logger = logger;
            _quotationService = quotationService;
            _customerService = customerService;
            _invoiceService = invoiceService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _quotationService.GetAllQuotationAsync();
            return View(viewModel);
        }

        // GET: QuotationController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _quotationService.GetQuotationDetail(id);

            ViewQuotationViewModel viewQuotationViewModel = new ViewQuotationViewModel()
            {
                Id = id,
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                DueDate = serviceModel.DueDate,
                Subtotal = serviceModel.Subtotal,
                Tax = serviceModel.Tax,
                ShippingFee = serviceModel.ShippingFee,
                Price = serviceModel.Price,
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
                Stocks = serviceModel?.Stocks,
            };
            return View(viewQuotationViewModel);
        }

        // GET: QuotationController/Create
        public async Task<IActionResult> Create()
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            AddQuotationViewModel addQuotationViewModel = new AddQuotationViewModel()
            {
                Date = DateTime.Now,
                DueDate = DateTime.Now,
                SerialNo = await _quotationService.GetLastestQuotationSerialNumberAsync()
            };

            AddQuotationWrapper addQuotationWrapper = new AddQuotationWrapper()
            {
                Merchant = documentMerchantDetail,
                ViewModel = addQuotationViewModel
            };
            return View(addQuotationWrapper);
        }

        // GET: QuotationController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var merchantServiceModel = await _customerService.GetMerchantDetailsAsync();
            DocumentCustomerDetailViewModel documentMerchantDetail = new DocumentCustomerDetailViewModel(merchantServiceModel.Id, merchantServiceModel.Name, merchantServiceModel.Address1, merchantServiceModel.Address2, merchantServiceModel.Postcode, merchantServiceModel.City, merchantServiceModel.State, merchantServiceModel.Phone, merchantServiceModel.Email);
            var serviceModel = await _quotationService.GetQuotationDetail(id);
            EditQuotationViewModel editQuotationViewModel = new EditQuotationViewModel()
            {
                SerialNo = serviceModel.SerialNo,
                CustomerId = serviceModel.Customer.Id,
                Date = serviceModel.Date,
                DueDate = serviceModel.DueDate,
                Subtotal = serviceModel.Subtotal,
                Tax = serviceModel.Tax,
                ShippingFee = serviceModel.ShippingFee,
                Price = serviceModel.Price,
                Stocks = serviceModel?.Stocks,
            };

            EditQuotationWrapper editQuotationWrapper = new EditQuotationWrapper
            {
                Merchant = documentMerchantDetail,
                Customer = serviceModel.Customer,
                ViewModel = editQuotationViewModel
            };
            ViewBag.CustomerId = serviceModel.Customer.Id;
            ViewBag.Id = id;
            return View(editQuotationWrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuotation([FromBody] QuotationRequestViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);
                viewModel.DueDate = TimeZoneInfo.ConvertTimeFromUtc(viewModel.DueDate, TimeZoneInfo.Local);

                EditQuotationServiceModel editQuotationServiceModel = new EditQuotationServiceModel()
                {
                    Id = viewModel.Id,
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    DueDate = viewModel.DueDate,
                    Subtotal = viewModel.Subtotal,
                    Tax = viewModel.Tax,
                    ShippingFee = viewModel.ShippingFee,
                    Price = viewModel.Price,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _quotationService.UpdateQuotationAsync(editQuotationServiceModel);
                TempData["Edit"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult AddQuotation([FromBody] AddQuotationWrapper wrapper)
        public async Task<IActionResult> AddQuotation([FromBody] QuotationRequestViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                viewModel.Date = TimeZoneInfo.ConvertTimeFromUtc(viewModel.Date, TimeZoneInfo.Local);
                viewModel.DueDate = TimeZoneInfo.ConvertTimeFromUtc(viewModel.DueDate, TimeZoneInfo.Local);

                AddQuotationServiceModel addQuotationServiceModel = new AddQuotationServiceModel()
                {
                    SerialNo = viewModel.SerialNo,
                    Date = viewModel.Date,
                    DueDate = viewModel.DueDate,
                    Subtotal = viewModel.Subtotal,
                    Tax = viewModel.Tax,
                    ShippingFee = viewModel.ShippingFee,
                    Price = viewModel.Price,
                    Stocks = viewModel.Stocks,
                    CustomerId = viewModel.CustomerId,
                };

                await _quotationService.CreateQuotationAsync(addQuotationServiceModel);

                TempData["Create"] = true;
                return Json(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return View();
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
                    await _quotationService.DeleteQuotationAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _quotationService.DeleteQuotationAsync(int.Parse(collection["Id"]));
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
        public async Task<IActionResult> TransferToInvoice(IFormCollection collection)
        {
            try
            {
                var serviceModel = await _quotationService.GetQuotationDetail(int.Parse(collection["Id"]));
                var stocks = serviceModel.Stocks;
                List<InvoiceStocksDetails>? invoiceStocksDetails = new List<InvoiceStocksDetails>();

                for(var i = 0; i < stocks.Count; i++)
                {
                    invoiceStocksDetails.Add(new InvoiceStocksDetails
                    {
                        Id = stocks[i].Id,
                        Description = stocks[i].Description,
                        Name = stocks[i].Name,
                        Quantity = stocks[i].Quantity,
                        UnitPrice = stocks[i].UnitPrice,
                    });
                }

                AddInvoiceServiceModel addInvoiceServiceModel = new AddInvoiceServiceModel()
                {
                    SerialNo = serviceModel.SerialNo,
                    Date = serviceModel.Date,
                    DueDate = serviceModel.DueDate,
                    Subtotal = serviceModel.Subtotal,
                    Tax = serviceModel.Tax,
                    ShippingFee = serviceModel.ShippingFee,
                    Price = serviceModel.Price,
                    Stocks = invoiceStocksDetails,
                    CustomerId = serviceModel.Customer.Id,
                };

                await _invoiceService.CreateInvoiceAsync(addInvoiceServiceModel);

                await _quotationService.ChangeQuotationStatusAsync(int.Parse(collection["Id"]));
                return RedirectToAction("Details", "Invoice", new { id = collection["Id"] });
            }
            catch
            {
                return View();
            }
        }
    }
}
