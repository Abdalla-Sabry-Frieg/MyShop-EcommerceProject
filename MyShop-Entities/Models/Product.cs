using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyShop_Entities.Models
{
    public class Product
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "Name is requires")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is requires")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Price is requires")]
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Image { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public List<ShoppingCart>shoppingCarts { get; set; }
    }
}
