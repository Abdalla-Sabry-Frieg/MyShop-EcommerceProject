using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyShop_Entities.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required(ErrorMessage ="Put your full  location ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Put your full name ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Put your city and street")]
        public string City { get; set; }
    }
}
