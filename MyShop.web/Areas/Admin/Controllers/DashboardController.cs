using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyShop_Entities.Helper;
using MyShop_Entities.Repositories;
using MyShop_Entities.ViewModels;
using System.Security.Claims;

namespace MyShop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Helpers.AdminRole)]
    public class DashboardController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ViewBag.Orders =_unitOfWork.Order.GetAll().Count();
            ViewBag.ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Products = _unitOfWork.Products.GetAll().Count();
            ViewBag.OrderAprve = _unitOfWork.Order.GetAll(x=>x.OrderStatus==Helpers.Approve).Count();

            return View();
        }
        [HttpGet]
        public IActionResult AprovedOrders()
        {

            var order = new OrderViewModel()
            {
              
                orders = _unitOfWork.Order.GetAll( x=>x.OrderStatus == Helpers.Approve ,includeWord: "ApplicationUser").ToList()

            };
            return View(order);
        }

    }
}
