using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [ValidateNever]
        public Order Order { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
    //    public List<ApplicationUser> Users { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
