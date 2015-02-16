﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVCEntityFramework.Models
{
    public class Order
    {
        public Order() { }
        [Key]
        public int OrderID { get; set; }
        [MaxLength(50),Required]
        public string CustomerID { get; set; }
        [DataType(DataType.Date), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        [DataType(DataType.Date), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequiredDate { get; set; }
        [Required]
        public DateTime ShippedDate { get; set; }
        [Required]
        public int ShipVia { get; set; }
        [DataType(DataType.Currency)]
        public decimal Freight { get; set; }
        [MaxLength(50), Required]
        public string ShipName { get; set; }
        [MaxLength(50), Required]
        public string ShipAddress { get; set; }
        [MaxLength(50), Required]
        public string ShipCity { get; set; }
        [MaxLength(50), Required]
        public string ShipRegion { get; set; }
        [MaxLength(50), Required]
        public string ShipPostalCode { get; set; }
        [MaxLength(50), Required]
        public string ShipCountry { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}