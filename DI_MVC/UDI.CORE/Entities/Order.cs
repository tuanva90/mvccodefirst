using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UDI.CORE.Entities
{
    public class Order
    {
        public Order() { }
        [Key]
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int ShipVia { get; set; }
        [DataType(DataType.Currency)]
        public decimal Freight { get; set; }
        [MaxLength(50)]
        public string ShipName { get; set; }
        [MaxLength(50)]
        public string ShipAddress { get; set; }
        [MaxLength(50)]
        public string ShipCity { get; set; }
        [MaxLength(50)]
        public string ShipRegion { get; set; }
        [MaxLength(50)]
        public string ShipPostalCode { get; set; }
        [MaxLength(50)]
        public string ShipCountry { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}