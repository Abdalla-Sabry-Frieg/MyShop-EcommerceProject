using MyShop_Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Repositories
{
    // Add spasific methodes that have a relation with category only
    public interface IOrderDetailsReposirory : IGenericRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
