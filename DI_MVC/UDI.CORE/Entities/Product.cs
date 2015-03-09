using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UDI.CORE.Entities
{
    public class Product
    {
        public Product() { }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        //[ForeignKey("CategoryID")]
        public int CategoryID { get; set; }
        //[ForeignKey("CategoryID")]
        //public Category Category { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public int Quantity { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}