using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Models
{
    public class ShoppingCart
    {
        [Key]
        public int? Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ValidateNever]
        // [AllowNull]
        public Product Product { get; set; }

        [ForeignKey("Applicationuser")]
        public string? ApplicationUserId { get; set; }

        [ValidateNever]
        // [AllowNull]
        public ApplicationUser Applicationuser {  get; set; }

        [Range(1, 100, ErrorMessage = "you must enter the number")]
        public int Count { get; set; }
    }
}
