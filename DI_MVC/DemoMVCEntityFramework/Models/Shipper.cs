using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Shipper
    {
        public Shipper() { }
        [Key]
        public int ShipperID { get; set; }
        [MaxLength(50)]
        public string CompanyName { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
    }
}