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
    public class ProductMapping : EntityTypeConfiguration<Product>
    {
        public ProductMapping()
        {
            HasKey(pro => pro.ProductID).HasRequired(c => c.Category).WithMany(s => s.Products).HasForeignKey(s => s.CategoryID);
            Property(pro => pro.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(pro => pro.CategoryID).IsRequired();
            Property(pro => pro.ProductName).IsRequired();
            Property(pro => pro.QuantityPerUnit).IsRequired();
            Property(pro => pro.UnitPrice).IsRequired();
            Property(pro => pro.UnitsInStock).IsRequired();
            Property(pro => pro.UnitsOnOrder).IsRequired();
            Property(pro => pro.ReorderLevel);
            Property(pro => pro.Discontinued);
            ToTable("Product");
        }
    }
}
