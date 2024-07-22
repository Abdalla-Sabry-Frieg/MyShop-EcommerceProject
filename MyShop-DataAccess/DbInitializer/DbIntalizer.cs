using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyShop_DataAccess.Data;
using MyShop_Entities.Helper;
using MyShop_Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.DbInitializer
{
    public class DbIntalizer : IDbIntalizer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly ApplicationDbContext _context;
        public DbIntalizer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManger,
            ApplicationDbContext context  )
        {
            _userManager = userManager;
            _roleManger = roleManger;
            _context = context;
        }

        public void Initalize()
        {
            // 1- migration
            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 1)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            // 2- Roles

            if (!_roleManger.RoleExistsAsync(Helpers.AdminRole).GetAwaiter().GetResult())
            {
                // to get the admin role as a default 
                _roleManger.CreateAsync(new IdentityRole(Helpers.AdminRole)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(Helpers.EditorRole)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(Helpers.CustomerRole)).GetAwaiter().GetResult();
            }


            // 3- User
            _userManager.CreateAsync(new ApplicationUser
                {
                    Name = "Adminstrator",
                    UserName = "admin@myshop.com",
                    Email = "admin@myshop.com",
                    Address="Abo-Hommous",
                    City="Cairo",
                },"P@ssword123"
            ).GetAwaiter().GetResult();

            var user = _context.ApplicationUsers.FirstOrDefault(x=>x.Email == "admin@myshop.com");

            _userManager.AddToRoleAsync(user,Helpers.AdminRole).GetAwaiter().GetResult();

            return;

        }

        
    }
}
