using BookyBook.DataAccess.Data;
using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using BookyBook.Models.ViewModels;
using BookyBook.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using BookyBook.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

	public class UserController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RoleManagement(string userId)
		{

			ManageUserVM manageUserVM = new()
			{
				//ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
				ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
				RoleList = _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }),
				CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
			};
			manageUserVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
	
            return View(manageUserVM);
		}
		[HttpPost]
        public IActionResult RoleManagement(ManageUserVM muVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == muVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == muVM.ApplicationUser.Id);
            if (!(muVM.ApplicationUser.Role == oldRole))
			{
				if(muVM.ApplicationUser.Role == SD.Role_Company)
				{
					applicationUser.CompanyId = muVM.ApplicationUser.CompanyId;

				}
				if(oldRole == SD.Role_Company)
				{
					applicationUser.CompanyId = null;
				}
				_unitOfWork.ApplicationUser.Update(applicationUser);
				_unitOfWork.Save();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, muVM.ApplicationUser.Role).GetAwaiter().GetResult();
			}
			else
			{
				if(oldRole == SD.Role_Company && applicationUser.CompanyId != muVM.ApplicationUser.CompanyId) {
					applicationUser.CompanyId = muVM.ApplicationUser.CompanyId;
					_unitOfWork.ApplicationUser.Update(applicationUser);
					_unitOfWork.Save();
				}
			}

            return RedirectToAction(nameof(Index));
        }

        #region apiCalls
        [HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company").ToList();
            foreach (ApplicationUser user in objUserList)
			{
				user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
				if(user.Company == null)
				{
					user.Company = new() { Name = "" };
				}
			}
			return Json(new { data = objUserList });
		}
		[HttpPost]
		public IActionResult lockUnlock([FromBody]string id)
		{
			ApplicationUser objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

			if(objFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else{
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
			_unitOfWork.ApplicationUser.Update(objFromDb);
			_unitOfWork.Save();

            return Json(new { success=true, message="Operation Successful" });
		}
		#endregion
	}
}
