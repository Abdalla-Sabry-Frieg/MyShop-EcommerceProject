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
    public class OrderRepository : GenericRepository<Order> , IOrderReposirory
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
           _context.Orders.Update(order);
        }

        public void UpdateOderStatus(int id, string? orderStatus, string? PaymentStatus)
        {
            var orderInDB = _context.Orders.FirstOrDefault(x=>x.Id == id);
            if (orderInDB != null)
            {
                orderInDB.OrderStatus = orderStatus;
                orderInDB.PaymentDate = DateTime.Now;
                if(PaymentStatus!= null)
                {
                    orderInDB.PaymentStatus = PaymentStatus;
                }
            }
        }
    }
}
