using Booky_Temp.Data;
using Booky_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Booky_Temp.Pages.Categories
{
	// [BindProperties]
    // you can also bind every properties to page by declaring bind property on top
	public class CreateModel : PageModel
    {
        private readonly AppDbCtx _db;
        [BindProperty]
        public Category? newCat { get; set; }

        public CreateModel(AppDbCtx db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
			_db.Categories.Add(newCat);
            _db.SaveChanges();
			TempData["success"] = "Category Created Successfully";
			return RedirectToPage("Index");
        }
		//public void OnPost(Category submittedCat)
		//{
		//    _db.Categories.Add(submittedCat);
		//    _db.SaveChanges();
		//}
	}
}
