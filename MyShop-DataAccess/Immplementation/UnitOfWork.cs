using MyShop_DataAccess.Data;
using MyShop_Entities.Models;
using MyShop_Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.Immplementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context) 
        {
            _context = context;
            Categories = new CategoryRepository(context);
            Products = new ProductRepository(context);
            ShoppingCart = new ShoppingCartRepository(context); 
            Order = new OrderRepository(context);
            OrderDetails = new OrderDetailsepository(context);
			ApplicationUser = new ApplicatioUserRepository(context);

		}
        public ICategoryReposirory Categories { get; private set; }

        public IProductRepository Products { get; private set; }

        public IShoppingCartReposirory ShoppingCart { get; private set; }

        public IOrderReposirory Order { get; private set; }

        public IOrderDetailsReposirory OrderDetails { get; private set; }

		public IApplicationUserRepository ApplicationUser { get; private set; }

		public int Complet()
        {
           return _context.SaveChanges();
        }

        public void Dispose()
        {
             _context.Dispose();
        }
    }
}
