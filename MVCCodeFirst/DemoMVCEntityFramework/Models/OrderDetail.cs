using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVCEntityFramework.Models
{
    public class OrderDetail
    {
        public OrderDetail() { }
        [Key, Column(Order = 1)]
        public int OrderID { get; set; }
        [Key,Column(Order = 2)]
        public int ProductID { get; set; }
        [DataType(DataType.Currency),Required]
        public decimal UnitPrice { get; set; }
        public int Quanlity { get; set; }
        public float Discount { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}