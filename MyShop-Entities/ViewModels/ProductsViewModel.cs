using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop_Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.ViewModels
{
    public class ProductsViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Product>? products { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
