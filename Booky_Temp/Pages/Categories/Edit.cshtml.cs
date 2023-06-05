using Booky_Temp.Data;
using Booky_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Booky_Temp.Pages.Categories
{
	[BindProperties]
	public class EditModel : PageModel
    {
		private readonly AppDbCtx _db;

		public Category? Category { get; set; }
		public EditModel(AppDbCtx db)
		{
			_db = db;
		}
		public void OnGet(int? id)
        {
			if(id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
        }
		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category);
				_db.SaveChanges();
				TempData["success"] = "Category Updated Successfully";

				return RedirectToPage("Index");
			}
			return Page();
		}
    }
}
