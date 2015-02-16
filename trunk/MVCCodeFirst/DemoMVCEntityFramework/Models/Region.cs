using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Region
    {
        public Region() { }
        [Key]
        public int RegionID { get; set; }
        [MaxLength(50)]
        public string RegionDescription { get; set; }
    }
}