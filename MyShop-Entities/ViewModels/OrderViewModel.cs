using MyShop_Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.ViewModels
{
    public class OrderViewModel
    {
        public Order? Order  { get; set; }
        public IEnumerable<OrderDetail> OrderDetails  { get; set; }
        public IEnumerable<Order> orders  { get; set; }
        public IEnumerable<ShoppingCart> CardList { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
