using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != 0 && id != null)
                category = _db.Categories.FirstOrDefault(c => c.Id == id);

        }
        public IActionResult OnPost()
        {
            Category categoryTodel = _db.Categories.Find(category.Id);
            _db.Categories.Remove(categoryTodel);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully.";
            //TempData["success"] = category.Name + " Category updated successfully.";
            return RedirectToPage("Index");

        }
    }
}
