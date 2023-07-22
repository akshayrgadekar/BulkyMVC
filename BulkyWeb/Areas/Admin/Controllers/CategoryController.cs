using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        
        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _UnitOfWork.Category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == Convert.ToString(category.DisplayOrder))
            {
                ModelState.AddModelError("Name", "The display order cannot exactly match the name.");
            }

            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Add(category);
                _UnitOfWork.Save();
                TempData["success"] = category.Name + " Category created successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _UnitOfWork.Category.Get(cat => cat.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Update(category);
                _UnitOfWork.Save();
                TempData["success"] = category.Name + " Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _UnitOfWork.Category.Get(cat => cat.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? category = _UnitOfWork.Category.Get(cat => cat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            string CategoryName = category.Name;
            _UnitOfWork.Category.Remove(category);
            _UnitOfWork.Save();
            TempData["success"] = CategoryName + " Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
