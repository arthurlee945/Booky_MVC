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
			List<BookyBook.Models.Product> objProdList = productRepo.GetAll(includeProperties: "Category").ToList();

			
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
				Product = new BookyBook.Models.Product()
			};
			if(id== null || id == 0)
			{
				return View(ProductVM);
			}
			else
			{
				ProductVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
				return View(ProductVM);
			}
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				if (productVM.Product.Id == 0)
				{
					productRepo.Add(productVM.Product);
				}
				else
				{
					productRepo.Update(productVM.Product);
				}
				_unitOfWork.Save();
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if(files != null)
				{

					foreach(IFormFile file in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\products\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);
						if (!Directory.Exists(finalPath)) 
							Directory.CreateDirectory(finalPath);
						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}
						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = productVM.Product.Id,
						};
						if(productVM.Product.ProductImages == null)
						{
							productVM.Product.ProductImages = new List<ProductImage>();
						}
						productVM.Product.ProductImages.Add(productImage);
					}
					_unitOfWork.Product.Update(productVM.Product);
					_unitOfWork.Save();
				}

				TempData["success"] = "Product Created/Updated Successfully";
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
		
		public IActionResult DeleteImage(int imageId)
		{
			var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
			int productId = imageToBeDeleted.ProductId;
			if(imageToBeDeleted != null)
			{
				if(!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
				{
					var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}
				_unitOfWork.ProductImage.Remove(imageToBeDeleted);
				_unitOfWork.Save();
				TempData["success"] = "Deleted successfully";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
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
			List<BookyBook.Models.Product> objProdList = productRepo.GetAll(includeProperties: "Category").ToList();
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
			//foreach(ProductImage img in productToBeDeleted.ProductImages)
			//{
			//	if (!string.IsNullOrEmpty(img.ImageUrl))
			//	{
			//		//delete
			//		var oldImagePath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('\\'));
			//		if (System.IO.File.Exists(oldImagePath))
			//		{
			//			System.IO.File.Delete(oldImagePath);
			//		}
			//	}
			//}
			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(webRootPath, productPath);
			if (!Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}
				Directory.Delete(finalPath);
			}


			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success=true, message="Delete Successful" });
		}
		#endregion
	}
}
