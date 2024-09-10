using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_Entities.Models;
using MyShop_Entities.Helper;
using MyShop_Entities.Repositories;
using System.Security.Claims;
using X.PagedList;
using System.Linq;
using MyShop_Entities.ViewModels;
using MyShop_DataAccess.Data;

namespace MyShop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public HomeController(IUnitOfWork unitOfWork , ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
             _context = context;
        }

        //public IActionResult Index(int? page = 1)
        //{

        //    var pageNumber = page ?? 1; // معناها انه يبدء من صفحه 1 
        //    int pageSize = 4;
        //    var products = _unitOfWork.Products.GetAll().ToPagedList(pageNumber, pageSize);
        //    return View(products);
        //}

        public IActionResult Index()
        {
            // Fetch all categories
            var categories = _unitOfWork.Categories.GetAll();

            // Create a list to hold the ViewModel instances
            var viewModel = new List<CategoryProductsViewModel>();

            foreach (var category in categories)
            {
                // Fetch the last 4 products for each category, ordered by creation date or any other criteria
                var products = _unitOfWork.Products
                                          .GetAll(p => p.CategoryId == category.Id)
                                          .OrderByDescending(p => p.CreatedAt)
                                          .Take(4)
                                          .ToList();

                // Add the category and its products to the ViewModel list
                viewModel.Add(new CategoryProductsViewModel
                {
                    Category = category,
                    Products = products
                });
            }
            // Pass the ViewModel list to the view
            return View(viewModel);
        }

        public IActionResult allProducts(int Id)
        {
            var products = _unitOfWork.Products.GetAll(x=>x.CategoryId == Id);
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
                    _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Sum(x=>x.Count));
                _unitOfWork.Complet();

            }
            else // if count add or remove 
            {
                _unitOfWork.ShoppingCart.InceaseCount(user, shopping.Count);
                _unitOfWork.Complet();
            }


          //  return RedirectToAction("Details");

             return   RedirectToAction("Index", "Cart");
        }

     
        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Handle empty search query (e.g., return a view with all products)
                var allProducts = _context.Products.ToList();
                return View("Search", allProducts);
            }

            // Perform the search
            var products = _context.Products
                                    .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                                    .ToList();

            return View("Search", products);
        }
    }

}

