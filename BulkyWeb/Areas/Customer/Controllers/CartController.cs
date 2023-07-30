using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVm shoppingCartVm { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVm = new ShoppingCartVm()
            {
                shoppingCartList = _unitOfWork.shoppingCart.GetAll(s => s.ApplicationUserId == userId, "product"),
                OrderHeader = new()
            };
            foreach (var cart in shoppingCartVm.shoppingCartList)
            {
                cart.price = GetPriceBaseOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal = (cart.price * cart.Count);
            }
            return View(shoppingCartVm);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVm = new ShoppingCartVm()
            {
                shoppingCartList = _unitOfWork.shoppingCart.GetAll(s => s.ApplicationUserId == userId, "product"),
                OrderHeader = new()
            };

            shoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.applicationUser.Get(a => a.Id == userId);

            shoppingCartVm.OrderHeader.Name = shoppingCartVm.OrderHeader.ApplicationUser.Name;
            shoppingCartVm.OrderHeader.PhoneNumber = shoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVm.OrderHeader.StreetAddress = shoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVm.OrderHeader.City = shoppingCartVm.OrderHeader.ApplicationUser.City;
            shoppingCartVm.OrderHeader.State = shoppingCartVm.OrderHeader.ApplicationUser.State;
            shoppingCartVm.OrderHeader.PostalCode = shoppingCartVm.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in shoppingCartVm.shoppingCartList)
            {
                cart.price = GetPriceBaseOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal = (cart.price * cart.Count);
            }
            return View(shoppingCartVm);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVm.shoppingCartList = _unitOfWork.shoppingCart.GetAll(s => s.ApplicationUserId == userId, "product");

            shoppingCartVm.OrderHeader.OrderDate = DateTime.Now;
            ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(a => a.Id == userId);


            foreach (var cart in shoppingCartVm.shoppingCartList)
            {
                cart.price = GetPriceBaseOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal = (cart.price * cart.Count);
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVm.OrderHeader.OrderStatus = SD.PaymentStatusPending;
            }
            else
            {
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusApprovedDelayedPayment;
                shoppingCartVm.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.orderHeader.Add(shoppingCartVm.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVm.shoppingCartList)
            {
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVm.OrderHeader.id,
                    Price = cart.price,
                    count = cart.Count
                };
                _unitOfWork.orderDeatils.Add(orderDetail);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Payment Logic
            }
            return RedirectToAction(nameof(OrderConfirmation),new { OrderId= shoppingCartVm.OrderHeader.id});

        }
        public IActionResult OrderConfirmation(int OrderId)
        {
            return View(OrderId);  
        }

        public IActionResult Plus(int? cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.shoppingCart.Get(S => S.Id == cartId);
            shoppingCart.Count += 1;
            _unitOfWork.shoppingCart.Update(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int? cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.shoppingCart.Get(S => S.Id == cartId);
            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.shoppingCart.Remove(shoppingCart);
            }
            else
            {
                shoppingCart.Count -= 1;
                _unitOfWork.shoppingCart.Update(shoppingCart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int? cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.shoppingCart.Get(S => S.Id == cartId);
            _unitOfWork.shoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBaseOnQuantity(ShoppingCart cart)
        {
            return cart.Count <= 50 ? cart.product.Price : cart.Count <= 100 ? cart.product.Price50 : cart.product.Price100;
        }
    }
}
