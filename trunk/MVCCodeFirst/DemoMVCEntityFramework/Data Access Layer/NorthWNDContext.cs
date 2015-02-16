using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DemoMVCEntityFramework.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DemoMVCEntityFramework.Data_Access_Layer
{
    public class NorthWNDContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //public IEnumerable<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
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
}