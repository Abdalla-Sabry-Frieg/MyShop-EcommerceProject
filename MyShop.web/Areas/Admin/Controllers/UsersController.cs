using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess.Data;
using MyShop_Entities.Helper;
using MyShop_Entities.Repositories;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyShop.web.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    [Authorize(Roles = Helpers.AdminRole)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UsersController(IUnitOfWork unitOfWork , ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public IActionResult Index()
        {
            // will show all users except the user is active right now

            var claimasIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimasIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userID = claim.Value;

            return View(_context.ApplicationUsers.Where(x=>x.Id != userID).ToList());
        }

         // to make admin lock or un lock any user
         // 1-  lockoutOnFailure: true => in login.cs make this true 
         // 2- and in prgram.cs the time as a  defualt time will be unlock
         // 
        public IActionResult LockUnLock(string? id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // if user not lock and admin will set it locked 
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow)
            {
                // will UnLocl after 6 monthes
                user.LockoutEnd = DateTime.UtcNow.AddMonths(6);
            }
            else // mean user is already locked
            {
                user.LockoutEnd = DateTime.UtcNow;
            }
            _context.SaveChanges();
            return RedirectToAction("Index","Users" , new {area = "Admin"});
        }
    }
}
