using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryReposirory Categories { get; }
        IProductRepository Products { get; }
        IShoppingCartReposirory ShoppingCart { get; }
        IOrderReposirory Order { get; }   
        IOrderDetailsReposirory OrderDetails { get; }
		IApplicationUserRepository ApplicationUser {  get; }

		int Complet();
    }
}
