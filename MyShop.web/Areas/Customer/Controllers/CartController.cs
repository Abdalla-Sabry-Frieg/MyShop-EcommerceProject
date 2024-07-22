using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_Entities.Helper;
using MyShop_Entities.Models;
using MyShop_Entities.Repositories;
using MyShop_Entities.ViewModels;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace MyShop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // get user id is active now from database using claims
            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = new ShoppingCartViewModel()
            {
                CardList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product"),
                Category = _unitOfWork.Categories.GetAll()
            };

            // to calculate the total price
            foreach (var item in cart.CardList)
            {
                cart.TotalPrice += (item.Count * item.Product.Price);
            }
            return View(cart);
        }

        public IActionResult plus(int cartid)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefualt(x => x.Id == cartid);
            _unitOfWork.ShoppingCart.InceaseCount(cart, 1);
            _unitOfWork.Complet();

            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count() +1;
            HttpContext.Session.SetInt32(Helpers.SessionKey, count);


            return RedirectToAction("Index");
             

        }
        public IActionResult minuis(int cartid)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefualt(x => x.Id == cartid);
            if(cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
                _unitOfWork.Complet();
                // و انا بحذف واحد م العناصر انزل من قيمه العناصر بتاعه اليوزر دا بمقدار واحد
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count() - 1;
                HttpContext.Session.SetInt32(Helpers.SessionKey, count);

            }
            else
            {
                _unitOfWork.ShoppingCart.DeceaseCount(cart, 1);
                _unitOfWork.Complet();
                // و انا بحذف واحد م العناصر انزل من قيمه العناصر بتاعه اليوزر دا بمقدار واحد
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count() - 1;
                HttpContext.Session.SetInt32(Helpers.SessionKey, count);
            }
          ;
            return RedirectToAction("Index");


        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefualt(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Complet();
            // بحذف العنصر كله ب كل عدده ف هجيب بقي العناصر احسبها و بعدين اعرضها
            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(Helpers.SessionKey, count);

            return RedirectToAction("Index" , "Cart");
        }
        [HttpGet]
        public IActionResult Summary()
        {
            // get user id is active now from database using claims
            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = new ShoppingCartViewModel()
            {
                CardList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product"),
                Category = _unitOfWork.Categories.GetAll(),
                Order = new Order(),
             //   Users = new ApplicationUser()

            };

            cart.Order.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefualt(x => x.Id == claim.Value);
            // Fill data in order at summary card 
            cart.Order.Name = cart.Order.ApplicationUser.Name;
            cart.Order.Email = cart.Order.ApplicationUser.Email;
            cart.Order.Address = cart.Order.ApplicationUser.Address;
            cart.Order.PhoneNumber = cart.Order.ApplicationUser.PhoneNumber; 
            cart.Order.City = cart.Order.ApplicationUser.City;  

            // to calculate the total price
            foreach (var item in cart.CardList)
            {
                cart.TotalPrice += (item.Count * item.Product.Price);
            }
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult Summary(ShoppingCartViewModel cart)
		{
			// get user id is active now from database using claims
			var claimasIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart.CardList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product");
            
            // Fill data at Strip 
           if(cart.Order == null )
            {
                cart.Order = new Order();

            }
            cart.Order.OrderStatus = Helpers.Pending;
            cart.Order.PaymentStatus = Helpers.Pending;
            cart.Order.OrderDate = DateTime.Now;
            cart.Order.UserId = claim.Value;
         


            // to calculate the total price
            foreach (var item in cart.CardList)
			{
				cart.TotalPrice += (item.Count * item.Product.Price);
			}
            cart.Order.TotalPrice = cart.TotalPrice;
            _unitOfWork.Order.Add(cart.Order);
            _unitOfWork.Complet();

            // Strip part "Payment"

            foreach (var item in cart.CardList)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = cart.Order.Id,
                    Price = item.Product.Price,
                    Count = item.Count,
                };
                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Complet();
            }

            // part of stripe
            var domain = "https://localhost:44317/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode="payment",
                SuccessUrl = domain+$"customer/cart/orderconfirmation?id={cart.Order.Id}",
                CancelUrl= domain+$"customer/cart/index",

            };

            foreach (var item in cart.CardList)
            {
                var sessionOptins = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionOptins);
            }


            var servies = new SessionService();
            Session session = servies.Create(options);
            cart.Order.SessionId = session.Id;
            cart.Order.OrderDate = DateTime.UtcNow;

            _unitOfWork.Complet();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        //    return View(cart);
		}
     
        public IActionResult OrderConfirmation(int id)
        {
            var order = _unitOfWork.Order.GetFirstOrDefualt(x => x.Id == id);
            var servce = new SessionService();
            Session session = servce.Get(order.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.Order.UpdateOderStatus(id, Helpers.Approve, Helpers.Approve);
                 order.PaymentIntentId = session.PaymentIntentId;
             
                _unitOfWork.Complet();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == order.UserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Complet();

            return View(id);
        }
    }
}
