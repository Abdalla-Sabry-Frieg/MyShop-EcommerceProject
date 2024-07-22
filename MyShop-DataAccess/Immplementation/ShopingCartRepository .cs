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
    public class ShoppingCartRepository : GenericRepository<ShoppingCart> , IShoppingCartReposirory
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int DeceaseCount(ShoppingCart cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }

        public int InceaseCount(ShoppingCart cart, int count)
        {
            cart.Count += count;
            return cart.Count;
        }
    }
}
