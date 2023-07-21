using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _UnitOfWork.Product.GetAll(includeProperties: nameof(Category)).ToList();
            
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _UnitOfWork.Category.GetAll().
                Select(u =>
                new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
                
            };
            if (id == null || id == 0)
            {
                return View(productVM);

            }
            else
            {
                productVM.Product = _UnitOfWork.Product.Get(p => p.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? productImage)
        {

            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (productImage != null)
                {
                    string fileName = Guid.NewGuid().ToString() +Path.GetExtension(productImage.FileName);
                    string productPath=Path.Combine(rootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(rootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStearm = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        productImage.CopyTo(fileStearm);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _UnitOfWork.Product.Update(productVM.Product);
                }

                _UnitOfWork.Save();
                TempData["success"] = productVM.Product.Title + " Product created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _UnitOfWork.Category.GetAll().
                Select(u =>
                new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            return View(productVM);

            }
        }

        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _UnitOfWork.Product.GetAll(includeProperties: nameof(Category)).ToList();

            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _UnitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            
            _UnitOfWork.Product.Remove(productToBeDeleted);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion Api Calls
    }
}
