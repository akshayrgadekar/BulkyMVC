using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Bulky.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CompanyController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> companies = _UnitOfWork.company.GetAll().ToList();
            return View(companies);
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = _UnitOfWork.company.Get(c => c.Id == id);
                return View(company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
                
                if (company.Id == 0)
                {
                    _UnitOfWork.company.Add(company);
                }
                else
                {
                    _UnitOfWork.company.Update(company);
                }

                _UnitOfWork.Save();
                TempData["success"] = company.Name + " Company created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }

        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companies = _UnitOfWork.company.GetAll().ToList();

            return Json(new { data = companies });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companiesToBeDeleted = _UnitOfWork.company.Get(u => u.Id == id);
            if (companiesToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _UnitOfWork.company.Remove(companiesToBeDeleted);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion Api Calls
    }
}
