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
    public class OrderDetailsepository : GenericRepository<OrderDetail> , IOrderDetailsReposirory
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail category)
        {
            _context.OrderDetails.Update(category);
        }
    }
}
