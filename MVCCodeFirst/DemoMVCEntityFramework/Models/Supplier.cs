using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Supplier
    {
        public Supplier() { }
        [Key]
        public int SupplierID { get; set; }
        [MaxLength(50)]
        public string CompanyName { get; set; }
        [MaxLength(50)]
        public string ContactName { get; set; }
        [MaxLength(50)]
        public string ContactTitle { get; set; }
        [MaxLength(50)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string Region { get; set; }
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(50)]
        public string Fax { get; set; }
        [MaxLength(50)]
        public string HomePage { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}