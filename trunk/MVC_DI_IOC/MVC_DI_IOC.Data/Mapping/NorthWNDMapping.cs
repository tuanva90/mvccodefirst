using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_DI_IOC.Core;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace MVC_DI_IOC.Data.Mapping
{
    public class CategoryMapping : EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            HasKey(cate => cate.ID);
            Property(cate => cate.ID).HasColumnName("CategoryID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(cate => cate.CategoryName).IsRequired().HasMaxLength(50);
            Property(cate => cate.Description).IsRequired().HasMaxLength(50);
            Property(cate => cate.Picture);
            ToTable("Category");
        }
    }
    public class ProductMapping : EntityTypeConfiguration<Product>
    {
        public ProductMapping()
        {
            HasKey(pro => pro.ID).HasRequired(c=>c.Category).WithMany(s=>s.Products).HasForeignKey(s=>s.CategoryID);
            Property(pro => pro.ID).HasColumnName("ProductID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
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
    public class CustomerMapping : EntityTypeConfiguration<Customer>
    {
        public CustomerMapping()
        {
            HasKey(c => c.ID).HasRequired(u => u.User).WithOptional(c => c.Customer);
            Property(c => c.ID).HasColumnName("CustomerID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.ContactName).IsRequired();
            Property(c => c.ContactTitle).IsRequired();
            Property(c => c.Address).IsRequired();
            Property(c => c.City).IsRequired();
            Property(c => c.Country).IsRequired();
            Property(c => c.Fax).IsRequired();
            Property(c => c.Phone).IsRequired();
            Property(c => c.Region ).IsRequired();
            Property(c => c.PostalCode).IsRequired();
            ToTable("Customer");
        }
    }
    public class UserMapping : EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            HasKey(u => u.ID);
            Property(u => u.ID).HasColumnName("CustomerID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(u => u.UserName).IsRequired();
            Property(u => u.PassWord).IsRequired();
            Property(u => u.Role).IsRequired();
            Property(u => u.Remember);
            ToTable("User");
        }
    }
    public class OrderMapping : EntityTypeConfiguration<Order>
    {
        public OrderMapping()
        {
            HasKey(o => o.ID).HasRequired(c => c.Customer).WithMany(o => o.Orders).HasForeignKey(c => c.CustomerID);
            Property(o => o.ID).HasColumnName("OrderID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(o => o.OrderDate).IsRequired();
            Property(o => o.RequireDate).IsRequired();
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

    public class OrderDetailMapping : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMapping()
        {
            HasKey(od => od.ID).HasRequired(o => o.Order).WithMany(od => od.OrderDetails);
            Property(od => od.ID).HasColumnName("OrderID");
            HasKey(od => od.ProductID).HasRequired(p => p.Product).WithMany(od => od.OrderDetails);
            Property(od => od.ProductID);
            Property(od => od.Quantity).IsRequired();
            Property(od => od.UnitPrice).IsRequired();
            Property(od => od.Discount);
            ToTable("OrderDetail");
        }
    }
}