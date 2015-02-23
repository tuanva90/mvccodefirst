using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DemoMVCEntityFramework.Data_Access_Layer;

namespace DemoMVCEntityFramework.Models
{
    public class Customer
    {
        public Customer() { }
        [Key]
        public int CustomerID { get; set; }
        [MaxLength(50), Required]
        public string CompanyName { get; set; }
        [MaxLength(50), Required]
        public string ContactName { get; set; }
        [MaxLength(50), Required]
        public string ContactTitle { get; set; }
        [MaxLength(60), Required]
        public string Address { get; set; }
        [MaxLength(50), Required]
        public string City { get; set; }
        [MaxLength(50), Required]
        public string Region { get; set; }
        [MaxLength(50), Required]
        public string PostalCode { get; set; }
        [MaxLength(50), Required]
        public string Country { get; set; }
        [MaxLength(20), Required]
        public string Phone { get; set; }
        [MaxLength(50), Required]
        public string Fax { get; set; }

        public int userID { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual User Users { get; set; }
        
    }
}