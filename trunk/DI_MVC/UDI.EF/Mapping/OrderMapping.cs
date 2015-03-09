using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.EF.Mapping
{
    public class OrderMapping : EntityTypeConfiguration<Order>
    {
        public OrderMapping()
        {
            HasKey(o => o.OrderID).HasRequired(c => c.Customer).WithMany(o => o.Orders).HasForeignKey(c => c.CustomerID);
            Property(o => o.OrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(o => o.OrderDate).IsRequired();
            Property(o => o.RequiredDate).IsRequired();
            Property(o => o.ShippedDate).IsRequired();
            Property(o => o.ShipVia).IsRequired();
            Property(o => o.Freight).IsRequired();
            Property(o => o.ShipAddress).IsRequired();
            Property(o => o.ShipCity).IsRequired();
            Property(o => o.ShipRegion).IsRequired();
            Property(o => o.ShipCountry).IsRequired();
            Property(o => o.ShipPostalCode).IsRequired();
            Property(o => o.ShipName).IsRequired();
            ToTable("Order");
        }
    }
}
