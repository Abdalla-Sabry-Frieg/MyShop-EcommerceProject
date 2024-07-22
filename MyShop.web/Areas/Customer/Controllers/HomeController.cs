using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_Entities.Models;
using MyShop_Entities.Helper;
using MyShop_Entities.Repositories;
using System.Security.Claims;
using X.PagedList;

namespace MyShop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page=1)
        {

            var pageNumber = page ?? 1; // معناها انه يبدء من صفحه 1 
            int pageSize = 4;
            var products = _unitOfWork.Products.GetAll().ToPagedList(pageNumber,pageSize);
            return View(products);
        }
        
        public IActionResult Details(int id) 
        {
            var cart = new ShoppingCart()
            {  
                ProductId = id,
                Product = _unitOfWork.Products.GetFirstOrDefualt(x => x.Id == id, includeWord: "Category"),
                Count = 1,


            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shopping)
        {
            // get user id is active now from database using claims
            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shopping.ApplicationUserId = claim.Value;
            shopping.Id = null;

            var user = new ShoppingCart();

            user = _unitOfWork.ShoppingCart.GetFirstOrDefualt(
                   x => x.ApplicationUserId == claim.Value && x.ProductId == shopping.ProductId);

            if (user == null) // means if no edit in the same cart 
            {
                _unitOfWork.ShoppingCart.Add(shopping);
                _unitOfWork.Complet();
                // key , value
                HttpContext.Session.SetInt32(Helpers.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());

            }
            else // if count add or remove 
            {
                _unitOfWork.ShoppingCart.InceaseCount(user, shopping.Count);
                _unitOfWork.Complet();
            }


            return RedirectToAction("Details");
        }
    }
}
