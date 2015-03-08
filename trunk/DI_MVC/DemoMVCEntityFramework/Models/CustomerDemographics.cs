using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class CustomerDemographics
    {
        public CustomerDemographics() { }
        [Key]
        public string CustomerTypeID { get; set; }
        [MaxLength(50)]
        public string CustomerDesc { get; set; }

        public virtual ICollection<CustomerCustomerDemo> CustomerCustomerDemos { get; set; }
    }
}