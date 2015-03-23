using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.EF.Mapping
{
    public class OrderDetailMapping : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMapping()
        {
            HasKey(od => new { od.OrderID, od.ProductID }).HasRequired(o => o.Order); //.WithMany(od => od.OrderDetails);
            Property(od => od.OrderID);
            //HasKey(od => od.ProductID).HasRequired(p => p.Product); //.WithMany(od => od.OrderDetails);
            Property(od => od.ProductID);
            Property(od => od.Quantity).IsRequired();
            Property(od => od.UnitPrice).IsRequired();
            Property(od => od.Discount);
            ToTable("OrderDetail");
        }
    }
}
