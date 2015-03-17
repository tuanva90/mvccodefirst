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
            Property(o => o.OrderDate);
            Property(o => o.RequiredDate);
            Property(o => o.ShippedDate);
            Property(o => o.ShipVia);
            Property(o => o.Freight);
            Property(o => o.ShipAddress);
            Property(o => o.ShipCity);
            Property(o => o.ShipRegion);
            Property(o => o.ShipCountry);
            Property(o => o.ShipPostalCode);
            Property(o => o.ShipName);
            ToTable("Order");
        }
    }
}
