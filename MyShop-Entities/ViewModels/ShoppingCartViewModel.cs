using MyShop_Entities.Models;
using MyShop_Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.ViewModels
{
    public class ShoppingCartViewModel
    {
      
      //  public string userId { get; set; }
        public IEnumerable<ShoppingCart> CardList {  get; set; }
        public ApplicationUser Users {  get; set; }
        public  IEnumerable<Category> Category { get; set; }
        public decimal TotalPrice { get; set; }
        public Order Order { get; set; }
        public string Address { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string PhoneNumber { get; set; }

    }
}
