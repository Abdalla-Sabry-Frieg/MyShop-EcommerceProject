using Microsoft.AspNetCore.Mvc;
using MyShop_Entities.Repositories;
using System.Security.Claims;
using MyShop_Entities.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.web.ViewComponunts
{
    // ViewComponent => like a partialView but this using when i want a big bessnis logic 
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(Helpers.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(Helpers.SessionKey));
                }
                else
                {
                    HttpContext.Session.SetInt32(Helpers.SessionKey, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
                    return View(HttpContext.Session.GetInt32(Helpers.SessionKey));

                }

            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
