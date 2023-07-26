using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public ShoppingCartVm   shoppingCartVm { get; set; }
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
                shoppingCartList = _unitOfWork.shoppingCart.GetAll(s => s.ApplicationUserId == userId, "product")
            };
            foreach (var cart in shoppingCartVm.shoppingCartList)
            {
                cart.price = GetPriceBaseOnQuantity(cart);
                shoppingCartVm.OrderTotal = (cart.price * cart.Count);
            }
            return View(shoppingCartVm);
        }

        public IActionResult Summary()
        {
            return View();
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
