using FSMS.Models;
using FSMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSMS.Controllers
{
    [Authorize]
    public class StocksController : Controller
    {
        private readonly ILogger<StocksController> _logger;
        private readonly IStocksService _stocksService;
        public StocksController(ILogger<StocksController> logger, IStocksService stocksService)
        {
            _logger = logger;
            _stocksService = stocksService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _stocksService.GetAllStocksAsync();
            return View(viewModel);
        }

        // GET: StocksController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var serviceModel = await _stocksService.GetStockDetailsAsync(id);
            ViewStockVIewModel viewStockVIewModel = new ViewStockVIewModel()
            {
                Id = serviceModel.Id,
                Description = serviceModel.Description,
                Name = serviceModel.Name,
                Picture = serviceModel.Picture,
                Price = serviceModel.Price,
                Quantity = serviceModel.Quantity
            };
            return View(viewStockVIewModel);
        }

        // GET: StocksController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StocksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if(!ModelState.IsValid) return View();

                var picture = collection.Files.GetFile("Picture");
                string fileName = String.Empty;

                if(picture != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    //get file extension
                    FileInfo fileInfo = new FileInfo(picture.FileName);
                    fileName = Guid.NewGuid().ToString() + fileInfo.Extension;

                    string fileNameWithPath = Path.Combine(path, fileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await picture.CopyToAsync(stream);
                    }
                }

                AddStocksServiceModel addStocksServiceModel = new AddStocksServiceModel
                {
                    Name = collection["Name"],
                    Description = collection["Description"],
                    Price = double.Parse(collection["Price"]),
                    Quantity = int.Parse(collection["Quantity"]),
                    Picture = fileName
                };

                await _stocksService.CreateStockAsync(addStocksServiceModel);

                TempData["Create"] = true;
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: StocksController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var serviceModel = await _stocksService.GetStockDetailsAsync(id);
                EditStockViewModel editStockViewModel = new EditStockViewModel
                {
                    Name = serviceModel.Name,
                    Description = serviceModel.Description,
                    Price = serviceModel.Price,
                    Quantity = serviceModel.Quantity
                };
                return View(editStockViewModel);
            }catch (Exception ex)
            {
                return View();
            }
            
        }

        // POST: StocksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                var picture = collection.Files.GetFile("Picture");
                string fileName = String.Empty;

                if (picture != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    //get file extension
                    FileInfo fileInfo = new FileInfo(picture.FileName);
                    fileName = Guid.NewGuid().ToString() + fileInfo.Extension;

                    string fileNameWithPath = Path.Combine(path, fileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await picture.CopyToAsync(stream);
                    }

                    //Used to delete old picture
                    #region Delete Old Picture
                    var currentProduct = await _stocksService.GetStockDetailsAsync(id);

                    if (currentProduct.Picture != null)
                    {
                        string currentFileName = currentProduct.Picture;
                        System.IO.File.Delete(Path.Combine(path, currentFileName));
                    }
                    #endregion
                }

                EditStockServiceModel serviceModel = new EditStockServiceModel()
                {
                    Id = id,
                    Description = collection["Description"],
                    Price = double.Parse(collection["Price"]),
                    Name = collection["Name"],
                    Picture = fileName,
                    Quantity = int.Parse(collection["Quantity"])
                };

                await _stocksService.EditStockAsync(serviceModel);

                TempData["Edit"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch
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
                    await _stocksService.DeleteStockAsync(int.Parse(collection["item.Id"]));
                }
                else
                {
                    await _stocksService.DeleteStockAsync(int.Parse(collection["Id"]));
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
        public async Task<IActionResult> RenderQuotationProductPartialView([FromBody] TestModel stockPartialViewModel)
        {
            List<StockPartialViewModel> models = stockPartialViewModel.models;
            List<QuotationStocksDetails> quotationStocksDetails1 = new List<QuotationStocksDetails>();
            //Run Query to Get All Information

            for (var i = 0; i < models.Count; i++)
            {
                var serviceModel = await _stocksService.GetStockDetailsAsync(models[i].Id);
                quotationStocksDetails1.Add(new QuotationStocksDetails()
                {
                    Id = models[i].Id,
                    Description = serviceModel.Description,
                    Name = serviceModel.Name,
                    UnitPrice = models[i].UnitPrice,
                    Quantity = models[i].Quantity
                });

            }
            return PartialView("_QuotationProductPartialView", quotationStocksDetails1);
        }

        [HttpPost]
        public async Task<IActionResult> RenderInvoiceProductPartialView([FromBody] TestModel stockPartialViewModel)
        {
            List<StockPartialViewModel> models = stockPartialViewModel.models;
            List<InvoiceStocksDetails> invoiceStocksDetails1 = new List<InvoiceStocksDetails>();
            //Run Query to Get All Information

            for (var i = 0; i < models.Count; i++)
            {
                var serviceModel = await _stocksService.GetStockDetailsAsync(models[i].Id);
                invoiceStocksDetails1.Add(new InvoiceStocksDetails()
                {
                    Id = models[i].Id,
                    Description = serviceModel.Description,
                    Name = serviceModel.Name,
                    UnitPrice = models[i].UnitPrice,
                    Quantity = models[i].Quantity
                });

            }
            return PartialView("_InvoiceProductPartialView", invoiceStocksDetails1);
        }

        [HttpPost]
        public async Task<IActionResult> RenderDeliveryOrderProductPartialView([FromBody] TestModel stockPartialViewModel)
        {
            List<StockPartialViewModel> models = stockPartialViewModel.models;
            List<DeliveryOrderStocksDetails> deliveryOrderStocksDetails1 = new List<DeliveryOrderStocksDetails>();
            //Run Query to Get All Information

            for (var i = 0; i < models.Count; i++)
            {
                var serviceModel = await _stocksService.GetStockDetailsAsync(models[i].Id);
                deliveryOrderStocksDetails1.Add(new DeliveryOrderStocksDetails()
                {
                    Id = models[i].Id,
                    Description = serviceModel.Description,
                    Name = serviceModel.Name,
                    Quantity = models[i].Quantity
                });

            }
            return PartialView("_DeliveryOrderProductPartialView", deliveryOrderStocksDetails1);
        }

        public async Task<IActionResult> GetStockDropDownData(string q)
        {
            List<StockDetalsServiceModel> stockDetalsServiceModels = await _stocksService.GetStockDropDownAsync(q);
            List<StockDropdownViewModel> stockDropdown = new List<StockDropdownViewModel>();
            if (stockDetalsServiceModels.Count > 0)
            {
                for (var i = 0; i < stockDetalsServiceModels.Count; i++)
                {
                    stockDropdown.Add(new StockDropdownViewModel(stockDetalsServiceModels[i].Id, stockDetalsServiceModels[i].Name, stockDetalsServiceModels[i].Description, stockDetalsServiceModels[i].Price));
                }
            }

            return Json(stockDropdown);
        }
    }
}
