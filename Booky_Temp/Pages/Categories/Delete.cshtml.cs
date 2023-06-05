using Booky_Temp.Data;
using Booky_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Booky_Temp.Pages.Categories
{
	[BindProperties]
	public class DeleteModel : PageModel
    {
		private readonly AppDbCtx _db;

		public Category? Category { get; set; }
		public DeleteModel(AppDbCtx db)
		{
			_db = db;
		}
		public void OnGet(int id)
		{
			Category = _db.Categories.Find(id);
		}
		public IActionResult OnPost()
		{
			if (Category == null)
			{
				return NotFound();
			}
			_db.Categories.Remove(Category);
			_db.SaveChanges();
			TempData["success"] = "Category Deleted Successfully";
			return RedirectToPage("Index");
		}
	}
}
