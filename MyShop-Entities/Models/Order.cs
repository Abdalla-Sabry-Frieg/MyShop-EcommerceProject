using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Models
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public string? Email { get; set; }
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; } // deleviery
        public DateTime PaymentDate { get; set; }

        // StripeSection Propirites

        public string? SessionId { get; set; }
        public string? SessionIntentId { get; set; }

        // Data Of User 

       
        public string? Address { get; set; }
   
        public string? Name { get; set; }
     
        public string? City { get; set; }
      
        public string? PhoneNumber { get; set; }

    }
}
