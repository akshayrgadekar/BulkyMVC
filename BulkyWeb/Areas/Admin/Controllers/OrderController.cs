using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
             orderVM = new()
            {
                orderHeader = _unitOfWork.orderHeader.Get(h => h.id == orderId, includeProperties: nameof(ApplicationUser)),
                orderDetails = _unitOfWork.orderDeatils.GetAll(d => d.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(h => h.id == orderVM.orderHeader.id);
            orderHeaderFromDb.Name = orderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.orderHeader.City;
            orderHeaderFromDb.State = orderVM.orderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.orderHeader.PostalCode;

            if (!string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
            {
            orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            }

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order updated successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.id });   
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderHeader.id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order updated successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(h => h.id == orderVM.orderHeader.id);
            orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;
            if(orderHeaderFromDb.PaymentStatus ==SD.PaymentStatusApprovedDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();            
            TempData["Success"] = "Order shipped successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(h => h.id == orderVM.orderHeader.id);
            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.orderHeader.UpdateStatus(orderHeaderFromDb.id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeaderFromDb.id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order cancelled successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Deatils_Pay_Now()
        {
            orderVM.orderHeader = _unitOfWork.orderHeader.Get(h => h.id == orderVM.orderHeader.id, 
                                 includeProperties: nameof(ApplicationUser));

            orderVM.orderDetails = _unitOfWork.orderDeatils.GetAll(d => d.OrderHeaderId == orderVM.orderHeader.id, 
                                 includeProperties: "Product");

            var domain = "https://localhost:7214/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentCofirmation?OrderId={orderVM.orderHeader.id}",
                CancelUrl = domain + $"admin/order/deatils?orderId={orderVM.orderHeader.id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderVM.orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.orderHeader.UpdateStripePaymentId(orderVM.orderHeader.id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentCofirmation(int OrderId)
        {

            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.id == OrderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApprovedDelayedPayment)
            {
                //this is an order by Company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStripePaymentId(OrderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(OrderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
           

            return View(OrderId);
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders=null;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.orderHeader.GetAll(includeProperties: nameof(ApplicationUser)).ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = _unitOfWork.orderHeader.GetAll(u => u.ApplicationUserId == userid,
                                includeProperties: nameof(ApplicationUser)).ToList();
            }
            switch (status)
            {
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o=>o.OrderStatus== SD.StatusInProcess);
                    break;
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);       
                    break;
                default:                     
                    break;
            }
            return Json(new { data = orderHeaders });
        }

        #endregion Api Calls
    }
}
