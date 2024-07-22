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
    public class CategoryRepository : GenericRepository<Category> , ICategoryReposirory
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryInDB = _context.Categories.FirstOrDefault(x=>x.Id == category.Id);  
            if (categoryInDB != null) 
            {
                categoryInDB.Description = category.Description;
                categoryInDB.Name = category.Name;
                categoryInDB.Image = category.Image;
                categoryInDB.CreatedAt = DateTime.Now;
            }
        }
    }
}
