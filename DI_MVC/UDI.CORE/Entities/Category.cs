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
        [Key]
        public int CategoryID { get; set; }
        [MaxLength(15),Required]
        public string CategoryName { get; set; }
        [MaxLength(500),Required]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}