using Booky_Temp.Data;
using Booky_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Booky_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbCtx _db;
        public List<Category>? CategoryList { get; set; }
        public IndexModel(AppDbCtx db)
        {
            _db = db;
        }

        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }
    }
}
