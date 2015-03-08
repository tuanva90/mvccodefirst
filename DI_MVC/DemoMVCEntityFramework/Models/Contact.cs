using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Contact
    {
        public Contact() { }
        [Key]
        public int ContactID { get; set; }
        [MaxLength(50)]
        public string ContactType { get; set; }
        [MaxLength(50)]
        public string CompanyName { get; set; }
        [MaxLength(50)]
        public string ContactName { get; set; }
        [MaxLength(50)]
        public string ContactTitle { get; set; }
        [MaxLength(60)]
        public string Address { get; set; }
        [MaxLength(15)]
        public string City { get; set; }
        [MaxLength(15)]
        public string Region { get; set; }
        [MaxLength(10)]
        public string PostalCode { get; set; }
        [MaxLength(15)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(4)]
        public string Extension { get; set; }
        [MaxLength(50)]
        public string Fax { get; set; }
        [MaxLength(50)]
        public string HomePage { get; set; }
        [MaxLength(255)]
        public string PhotoPath { get; set; }
        public byte[] Photo { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}