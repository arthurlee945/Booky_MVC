using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using BookyBook.Models.ViewModels;
using BookyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BookyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductRepository productRepo;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = db;
			_webHostEnvironment = webHostEnvironment;
			productRepo = _unitOfWork.Product;
		}
		public IActionResult Index()
		{
			List<Product> objProdList = productRepo.GetAll(includeProperties: "Category").ToList();

			
			return View(objProdList);
		}

		public IActionResult Upsert(int? id)
		{

			ProductVM ProductVM = new ProductVM() {
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem()
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};
			if(id== null || id == 0)
			{
				return View(ProductVM);
			}
			else
			{
				ProductVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(ProductVM);
			}
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if(file != null)
				{
					string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\products");
					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//delete
						string oldPath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldPath))
						{
							System.IO.File.Delete(oldPath);
						}
					}
					using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\products\" + fileName;
				}
				if(productVM.Product.Id == 0)
				{
					productRepo.Add(productVM.Product);
				}
				else
				{
					productRepo.Update(productVM.Product);
				}
				_unitOfWork.Save();
				TempData["success"] = "Product Created Successfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem()
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
				return View(productVM);
			}

		}
		
		//public IActionResult Delete(int? id)
		//{
		//	if (id == null || id == 0)
		//	{
		//		return NotFound();
		//	}
		//	Product? productFromDb = productRepo.Get(u => u.Id == id);
		//	if (productFromDb == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(productFromDb);
		//}
		//[HttpPost, ActionName("Delete")]

		//public IActionResult DeletePost(int? id)
		//{
		//	Product? deletingPost = productRepo.Get(u => u.Id == id);
		//	if (deletingPost == null)
		//	{
		//		return NotFound();
		//	}
		//	productRepo.Remove(deletingPost);
		//	_unitOfWork.Save();
		//	TempData["success"] = "Product Deleted Successfully";

		//	return RedirectToAction("Index");
		//}

		#region apiCalls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProdList = productRepo.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProdList });
		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
			if(productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}
			string webRootPath = _webHostEnvironment.WebRootPath;
	
			if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
			{
				//delete
				var oldImagePath = Path.Combine(webRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
				if (System.IO.File.Exists(oldImagePath))
				{
					System.IO.File.Delete(oldImagePath);
				}
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success=true, message="Delete Successful" });
		}
		#endregion
	}
}
