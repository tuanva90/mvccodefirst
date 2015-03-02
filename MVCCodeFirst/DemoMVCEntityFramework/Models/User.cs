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
        [Required(ErrorMessage="User Name must containt at least 3 characters and max is 20 characters!"),MinLength(3),MaxLength(20)]
        public string UserName { get; set; }
        [Required(ErrorMessage="Pass must containt at least 6 characters and max is 14 characters!")]
        [MinLength(6)]
        [MaxLength(14)]
        public string Password { get; set; }
        [EmailAddress(ErrorMessage="Invalid Email!")]
        //[DataType(DataType.EmailAddress)]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required,MaxLength(50)]
        public string Email { get; set; }
        [Display(Name="Remember Me")]
        public bool Bool { get; set; }
        [NotMapped, Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]        
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