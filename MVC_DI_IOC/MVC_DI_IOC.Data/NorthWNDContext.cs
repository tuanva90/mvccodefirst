using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection; 
using System.Web;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.Data.Entity.ModelConfiguration.Conventions;
using MVC_DI_IOC.Data.Mapping;

namespace MVC_DI_IOC.Data
{
    public class NorthWNDContext : DbContext
    {
        public NorthWNDContext() : base("name=NorthWNDContext1")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<NorthWNDContext>());
        }

        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new CategoryMapping());
            modelBuilder.Configurations.Add(new ProductMapping());
            modelBuilder.Configurations.Add(new CustomerMapping());
            modelBuilder.Configurations.Add(new UserMapping());
            modelBuilder.Configurations.Add(new OrderMapping());
            modelBuilder.Configurations.Add(new OrderDetailMapping());
        }
    }
}