using Microsoft.AspNetCore.Mvc;
using MyShop_Entities.Repositories;
using MyShop_Entities.ViewModels;
using MyShop_Entities.Models;
using System.Security.Claims;
using MyShop_Entities.Helper;
using Stripe;

namespace MyShop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        //Data Bind  using at update
        [BindProperty]
        public OrderViewModel model { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var order = new OrderViewModel()
            {
                Order = new Order(),
                CardList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product"),
                orders = _unitOfWork.Order.GetAll(includeWord: "ApplicationUser").ToList()

            };
            // to calculate the total price
            foreach (var item in order.CardList)
            {
                order.TotalPrice += (item.Count * item.Product.Price);
            }
            return View(order);
        }
       

        // Action to return data to data table as json
        [HttpGet]
        public IActionResult GetData()
        {
            var Orders = _unitOfWork.Order.GetAll(includeWord: "ApplicationUser");
            return Json(new { data = Orders });
        }

        [HttpGet]
        public IActionResult Details(int orderId)
        {
            var order = new OrderViewModel()
            {
                Order = _unitOfWork.Order.GetFirstOrDefualt(x=>x.Id == orderId , includeWord: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(x=>x.OrderId == orderId , includeWord:"Product")
            };

           return View(order);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult updateOrderDetails()
        {
            var order = _unitOfWork.Order.GetFirstOrDefualt(x => x.Id == model.Order.Id);

            order.Name = model.Order.Name;
            order.PhoneNumber = model.Order.PhoneNumber;
            order.Address = model.Order.Address;
            order.City = model.Order.City;

            if(model.Order.Carrier !=null)
            {
                order.Carrier = model.Order.Carrier;
            }
            if (model.Order.TrackingNumber != null)
            {
                order.TrackingNumber = model.Order.TrackingNumber;
            }
               

                _unitOfWork.Order.Update(order);
                _unitOfWork.Complet();

                TempData["Edit"] = "Order has been updated";
                return RedirectToAction("Details", "Order", new { orderId = order.Id });

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcess()
        {
            _unitOfWork.Order.UpdateOderStatus(model.Order.Id, Helpers.Procceing, null);
            _unitOfWork.Complet();

            TempData["Edit"] = "Order Status has been updated";
            return RedirectToAction("Details", "Order", new { orderId = model.Order.Id });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var order = _unitOfWork.Order.GetFirstOrDefualt(x => x.Id == model.Order.Id);
            order.TrackingNumber = model.Order.TrackingNumber;
            order.Carrier = model.Order.Carrier;
            order.OrderStatus = Helpers.Shipped;
            order.ShippingDate = DateTime.Now;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Complet();

            TempData["Edit"] = "Order has been Shipped";
            return RedirectToAction("Details", "Order", new { orderId = model.Order.Id });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var order = _unitOfWork.Order.GetFirstOrDefualt(x => x.Id == model.Order.Id);
           
            if(order.PaymentStatus == Helpers.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId,
                };
                var service = new RefundService();
                Refund refund = service.Create(option);

                _unitOfWork.Order.UpdateOderStatus(order.Id, Helpers.Canceled, Helpers.Refund);

            }
            else // لو الفلوس مدفوعخ ترجع لو لا يبقي يكنسل و خلاص
            {
                _unitOfWork.Order.UpdateOderStatus(order.Id, Helpers.Canceled, Helpers.Canceled);
            }

          
            _unitOfWork.Complet();

            TempData["Edit"] = "Order has been Canceld";
            return RedirectToAction("Details", "Order", new { orderId = model.Order.Id });


        }
    }
}
