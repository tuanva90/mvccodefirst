using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DemoMVCEntityFramework.Data_Access_Layer;

namespace DemoMVCEntityFramework.Models
{
    public class User
    {
        [Key]
        public int CustomerID { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage="Pass must containt at least 6 characters and max is 14 characters!")]
        [MinLength(6)]
        [MaxLength(14)]
        public string Password { get; set; }
        [Display(Name="Remember Me")]
        public bool Bool { get; set; }
        [NotMapped, Compare("Password")]        
        public string ConfirmPassword { get; set; }
        [Required]
        public bool Roles { get; set; }
        public virtual Customer Customer { get; set; }

        public bool IsValid(string _username, string _password)
        {
            NorthWNDContext db = new NorthWNDContext();
            var check = from p in db.Users where p.UserName == _username && p.Password == _password select p.UserName;
            if (check.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}