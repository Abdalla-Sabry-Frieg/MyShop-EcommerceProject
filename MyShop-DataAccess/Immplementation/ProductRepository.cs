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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productInDB = _context.Products.FirstOrDefault(x=>x.Id == product.Id);
            if (productInDB != null) 
            {
                productInDB.Name= product.Name;
                productInDB.Description= product.Description;
                productInDB.Price= product.Price;
                productInDB.Image= product.Image;
                productInDB.CreatedAt= DateTime.Now;
                productInDB.CategoryId= product.CategoryId;
            }
           
        }
    }
}
