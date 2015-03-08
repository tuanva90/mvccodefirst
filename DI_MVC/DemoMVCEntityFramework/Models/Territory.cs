using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Territory
    {
        public Territory() { }
        [Key]
        public string TerritotyID { get; set; }
        [MaxLength(50)]
        public string TerritoryDescription { get; set; }
        //Foreign key for Region
        public int RegionID { get; set; }
        public Region Region { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}