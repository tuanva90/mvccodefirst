using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UDI.CORE.Entities
{
    public class Customer
    {
        public Customer() { }
        public int CustomerID { get; set; }
        [StringLength(20),MinLength(3)]
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        [RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "PostalCode should contain only numbers")]
        public string PostalCode { get; set; }
        public string Country { get; set; }
        [RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "PhoneNumber should contain only numbers")]
        public string Phone { get; set; }
        [RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "Fax should contain only numbers")]
        public string Fax { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual User User { get; set; }
    }
}