using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UDI.CORE.Entities;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace UDI.EF.DAL
{
    public interface IEFContext
    {

    }

    public class EFContext : DbContext, IEFContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //public IEnumerable<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<Supplier> Suppliers { get; set; }
        //public DbSet<Region> Regions { get; set; }
        //public DbSet<Shipper> Shippers { get; set; }
        //public DbSet<Territory> Territories { get; set; }
        //public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class TestInitializer : CreateDatabaseIfNotExists<EFContext>
    {
        protected override void Seed(EFContext context)
        {
            var categories = new List<Category>
            {
                new Category{CategoryName="Laptop",Description="Laptop"},
                new Category{CategoryName="Mobile",Description="Mobile"},
                new Category{CategoryName="HeadPhone",Description="HeadPhone"}
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var products = new List<Product>
            {
                new Product{ProductName="Dell N5110 Inspiron Core-i5",Category = categories.Find(c => c.CategoryName == "Laptop"),
                    QuantityPerUnit="1 per unit",UnitPrice=100,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=2,ProductName="Dell N4110 Inspiron Core-i3",Category = categories.Find(c => c.CategoryName == "Laptop"),
                    QuantityPerUnit="1 per unit",UnitPrice=80,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=3,ProductName="Asus Zenfone 5",Category = categories.Find(c => c.CategoryName == "Mobile"),
                    QuantityPerUnit="1 per unit",UnitPrice=40,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=4,ProductName="HP Pavilion Core-i7",Category = categories.Find(c => c.CategoryName == "Laptop"),
                    QuantityPerUnit="1 per unit",UnitPrice=120,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=5,ProductName="HP Probook Core-i7",Category = categories.Find(c => c.CategoryName == "Laptop"),
                    QuantityPerUnit="1 per unit",UnitPrice=150,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            };
            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();
        }
    }
}