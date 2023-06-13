using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using BookyBook.Models.ViewModels;
using BookyBook.Utility;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;

        }
        public IActionResult Index()
        {

            ClaimsIdentity CI = (ClaimsIdentity)User.Identity;
            string userId = CI.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM = new() {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (ShoppingCart c in shoppingCartVM.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
				shoppingCartVM.OrderHeader.OrderTotal += c.Price * c.Count;
            }
            return View(shoppingCartVM);
        }
        public IActionResult Plus(int cartId) {
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);

            if (cartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
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
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
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
            ClaimsIdentity CI = (ClaimsIdentity)User.Identity;
            string userId = CI.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
			shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (ShoppingCart c in shoppingCartVM.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
                shoppingCartVM.OrderHeader.OrderTotal += c.Price * c.Count;
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            ClaimsIdentity CI = (ClaimsIdentity)User.Identity;
            string userId = CI.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");

            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            foreach (ShoppingCart c in shoppingCartVM.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
                shoppingCartVM.OrderHeader.OrderTotal += c.Price * c.Count;
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is regular to capture payment
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (ShoppingCart c in shoppingCartVM.ShoppingCartList)
            {
                _unitOfWork.OrderDetail.Add(new OrderDetail() {
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    ProductId = c.ProductId,
                    Count = c.Count,
                    Price = c.Price
                });
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is regular Stripe logic
                string domain = @"http://localhost:5243/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl= domain+"customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

                //options.LineItems = (List<SessionLineItemOptions>)shoppingCartVM.ShoppingCartList.Select(item => {
                //    return new SessionLineItemOptions()
                //    {
                //        PriceData = new SessionLineItemPriceDataOptions
                //        {
                //            UnitAmount = (long)(item.Price * 100),
                //            Currency = "usd",
                //            ProductData = new SessionLineItemPriceDataProductDataOptions
                //            {
                //                Name = item.Product.Title
                //            }
                //        },
                //        Quantity = item.Count
                //    };
                //});
                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
				Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}
            return RedirectToAction(nameof(OrderConfirmation), new {id = shoppingCartVM.OrderHeader.Id});
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader oh = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
      

            if(oh.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(oh.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
				}
            }
            HttpContext.Session.Clear();
            _emailSender.SendEmailAsync(oh.ApplicationUser.Email, "New Order - Bulky Book", $"<p>New Order created - {oh.Id}</p>");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == oh.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }
	}
}
