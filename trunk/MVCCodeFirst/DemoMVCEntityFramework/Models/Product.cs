using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVCEntityFramework.Models
{
    public class Product
    {
        public Product() { }
        [Key]
        public int ProductID { get; set; }
        [MaxLength(50), Required]
        public string ProductName { get; set; }
        //[ForeignKey("CategoryID")]
        [Required]
        public int CategoryID { get; set; }
        //[ForeignKey("CategoryID")]
        //public Category Category { get; set; }
        [MaxLength(50), Required]
        public string QuantityPerUnit { get; set; }
        [DataType(DataType.Currency),Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public int UnitsInStock { get; set; }
        [Required]
        public int UnitsOnOrder { get; set; }
        [Required]
        public int ReorderLevel { get; set; }
        [Required]
        public bool Discontinued { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}