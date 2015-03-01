namespace DemoMVCEntityFramework.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using DemoMVCEntityFramework.Models;
    using DemoMVCEntityFramework.Data_Access_Layer;

    internal sealed class Configuration : DbMigrationsConfiguration<NorthWNDContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NorthWNDContext context)
        {
            //var categories = new List<Category>
            //{
            //    new Category{CategoryID=1,CategoryName="Laptop",Description="Laptop"},
            //    new Category{CategoryID=2,CategoryName="Mobile",Description="Mobile"},
            //    new Category{CategoryID=3,CategoryName="HeadPhone",Description="HeadPhone"}
            //};
            //categories.ForEach(c => context.Categories.AddOrUpdate(n => n.CategoryName, c));
            //context.SaveChanges();
            //var suppliers = new List<Supplier>
            //{
            //    new Supplier{SupplierID=1,CompanyName="Dell",ContactName="Dell",ContactTitle="Dell branch",Address="Round Rock TX",City="Texas",Region="NA",PostalCode="",Country="America",Phone="",Fax="",HomePage="www.dell.com"},
            //    new Supplier{SupplierID=1,CompanyName="Apple",ContactName="Apple",ContactTitle="Apple branch",Address="Cupertino CA",City="California",Region="NA",PostalCode="",Country="America",Phone="",Fax="",HomePage="www.apple.com"},
            //};

            //var products = new List<Product>
            //{
            //    new Product{ProductID=1,ProductName="Laptop Dell Inspiron",SupplierID=1,CategoryID=1,QuantityPerUnit="1 per unit",UnitPrice=100}
            //};
        }
    }
    public class TestInitializer : DropCreateDatabaseIfModelChanges<NorthWNDContext>
    {
        protected override void Seed(NorthWNDContext context)
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
                new Product{ProductName="Dell N5110 Inspiron Core-i5",CategoryID=1,
                    QuantityPerUnit="1 per unit",UnitPrice=100,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=2,ProductName="Dell N4110 Inspiron Core-i3",CategoryID=1,
                    QuantityPerUnit="1 per unit",UnitPrice=80,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=3,ProductName="Asus Zenfone 5",CategoryID=2,
                    QuantityPerUnit="1 per unit",UnitPrice=40,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=4,ProductName="HP Pavilion Core-i7",CategoryID=1,
                    QuantityPerUnit="1 per unit",UnitPrice=120,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
                new Product{ProductID=5,ProductName="HP Probook Core-i7",CategoryID=1,
                    QuantityPerUnit="1 per unit",UnitPrice=150,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            };
            products.ForEach(p => context.Products.AddOrUpdate( pro => pro.ProductID, p));
            context.SaveChanges();
        }
    }
}
