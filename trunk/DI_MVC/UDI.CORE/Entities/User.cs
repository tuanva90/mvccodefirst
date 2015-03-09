using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UDI.CORE.Entities
{
    public class User
    {
        public int CustomerID { get; set; }
        [MaxLength(20),MinLength(3)]
        public string UserName { get; set; }
        [MaxLength(50),MinLength(3)]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage="Invalid Email!")]       
        public string Email { get; set; }
        [Display(Name="Remember me")]
        public bool Bool { get; set; }
        //[NotMapped, Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]   
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public bool Roles { get; set; }
        public virtual Customer Customer { get; set; }
    }
}