using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductRepository productRepo;
		public ProductController(IUnitOfWork db)
		{
			_unitOfWork = db;
			productRepo = _unitOfWork.Product;
		}
		public IActionResult Index()
		{
			List<Product> objCatList = productRepo.GetAll().ToList();
			return View(objCatList);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Product obj)
		{
			if (ModelState.IsValid)
			{
				productRepo.Add(obj);
				_unitOfWork.Save();
				TempData["success"] = "Product Created Successfully";
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
			Product? productFromDb = productRepo.Get(u => u.Id == id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost]
		public IActionResult Edit(Product obj)
		{
			if (ModelState.IsValid)
			{
				productRepo.Update(obj);
				_unitOfWork.Save();
				TempData["success"] = "Product Updated Successfully";

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
			Product? productFromDb = productRepo.Get(u => u.Id == id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost, ActionName("Delete")]

		public IActionResult DeletePost(int? id)
		{
			Product? deletingPost = productRepo.Get(u => u.Id == id);
			if (deletingPost == null)
			{
				return NotFound();
			}
			productRepo.Remove(deletingPost);
			_unitOfWork.Save();
			TempData["success"] = "Product Deleted Successfully";

			return RedirectToAction("Index");
		}
	}
}
