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

	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICompanyRepository companyRepo;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public CompanyController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = db;
			_webHostEnvironment = webHostEnvironment;
			companyRepo = _unitOfWork.Company;
		}
		public IActionResult Index()
		{
			List<Company> objProdList = companyRepo.GetAll().ToList();

			
			return View(objProdList);
		}

		public IActionResult Upsert(int? id)
		{

			if(id== null || id == 0)
			{
				return View(new Company());
			}
			else
			{
				Company company = _unitOfWork.Company.Get(u => u.Id == id);
				return View(company);
			}
		}
		[HttpPost]
		public IActionResult Upsert(Company companyObj)
		{
			if (ModelState.IsValid)
			{

				if(companyObj.Id == 0)
				{
					companyRepo.Add(companyObj);
				}
				else
				{
					companyRepo.Update(companyObj);
				}
				_unitOfWork.Save();
				TempData["success"] = "Company Created Successfully";
				return RedirectToAction("Index");
			}
			else
			{
				return View(companyObj);
			}

		}
		
		//public IActionResult Delete(int? id)
		//{
		//	if (id == null || id == 0)
		//	{
		//		return NotFound();
		//	}
		//	Company? productFromDb = companyRepo.Get(u => u.Id == id);
		//	if (productFromDb == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(productFromDb);
		//}
		//[HttpPost, ActionName("Delete")]

		//public IActionResult DeletePost(int? id)
		//{
		//	Company? deletingPost = companyRepo.Get(u => u.Id == id);
		//	if (deletingPost == null)
		//	{
		//		return NotFound();
		//	}
		//	companyRepo.Remove(deletingPost);
		//	_unitOfWork.Save();
		//	TempData["success"] = "Company Deleted Successfully";

		//	return RedirectToAction("Index");
		//}

		#region apiCalls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> objProdList = companyRepo.GetAll().ToList();
			return Json(new { data = objProdList });
		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
			if(companyToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			_unitOfWork.Company.Remove(companyToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success=true, message="Delete Successful" });
		}
		#endregion
	}
}
