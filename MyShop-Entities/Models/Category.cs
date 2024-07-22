using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyShop_Entities.Models
{
    public class Category
    {
        [Key]
        public int? Id { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Name is requires")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is requires")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        [ValidateNever]
        public Category Parent { get; set; }
        [ValidateNever]
        public List<Product> Products { get; set; } 
       

    }
}
