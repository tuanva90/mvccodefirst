using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class CustomerCustomerDemo
    {
        public CustomerCustomerDemo() { }
        [Key]
        public string CustomerID { get; set; }
        //Foreign key for CustomerDemographics
        [MaxLength(50)]
        public string CustomerTypeID { get; set; }
        public CustomerDemographics CustomerDemographics { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}