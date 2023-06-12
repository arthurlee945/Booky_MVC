using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using BookyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ClaimsIdentity CI = (ClaimsIdentity)User.Identity;
            string userId = CI.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new () { 
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderTotal = 0
            };
            foreach(ShoppingCart c in ShoppingCartVM.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
                ShoppingCartVM.OrderTotal += c.Price * c.Count;
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int cartId) {
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count +=1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

            if(cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int cartId)
        {
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double GetPriceBasedOnQuantity(ShoppingCart sc)
        {
            return sc.Count <= 50 ? sc.Product.Price : sc.Count <= 100 ? sc.Product.Price50 : sc.Product.Price100; 
        }

        public IActionResult Summary()
        {
            return View();
        }
    }
}
