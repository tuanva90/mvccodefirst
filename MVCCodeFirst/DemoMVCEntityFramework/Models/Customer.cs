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
        [Key,ForeignKey("User")]
        public int CustomerID { get; set; }
        [MaxLength(50)]
        public string ContactName { get; set; }
        [MaxLength(50)]
        public string ContactTitle { get; set; }
        [MaxLength(60)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string Region { get; set; }
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(50)]
        public string Fax { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual User User { get; set; }
    }
}