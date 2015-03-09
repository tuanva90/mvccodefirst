using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UDI.CORE.Entities
{
    public class Category
    {
        public Category() 
        {
            
        }
        public int CategoryID { get; set; }
        [Required, MinLength(3),MaxLength(20)]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}