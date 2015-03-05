using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_DI_IOC.Web.App_Start
{
    public class Configuration
    {

    }
    public class TestInitializer : CreateDatabaseIfNotExists<NorthWNDContext>
    {
        protected static void Seed(NorthWNDContext context)
        {
            var categories = new List<Category>
            {
                new Category{CategoryName="Laptop",Description="Laptop"},
                new Category{CategoryName="Mobile",Description="Mobile"},
                new Category{CategoryName="HeadPhone",Description="HeadPhone"}
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            //var products = new List<Product>
            //{
            //    new Product{ProductName="Dell N5110 Inspiron Core-i5",Category = categories.Find(c => c.CategoryName == "Laptop"),
            //        QuantityPerUnit="1 per unit",UnitPrice=100,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            //    new Product{ProductID=2,ProductName="Dell N4110 Inspiron Core-i3",Category = categories.Find(c => c.CategoryName == "Laptop"),
            //        QuantityPerUnit="1 per unit",UnitPrice=80,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            //    new Product{ProductID=3,ProductName="Asus Zenfone 5",Category = categories.Find(c => c.CategoryName == "Mobile"),
            //        QuantityPerUnit="1 per unit",UnitPrice=40,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            //    new Product{ProductID=4,ProductName="HP Pavilion Core-i7",Category = categories.Find(c => c.CategoryName == "Laptop"),
            //        QuantityPerUnit="1 per unit",UnitPrice=120,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            //    new Product{ProductID=5,ProductName="HP Probook Core-i7",Category = categories.Find(c => c.CategoryName == "Laptop"),
            //        QuantityPerUnit="1 per unit",UnitPrice=150,UnitsInStock=1,UnitsOnOrder=1,ReorderLevel=1,Discontinued=true},
            //};
            //products.ForEach(p => context.Products.AddOrUpdate(pro => pro.ProductID, p));
            //context.SaveChanges();
        }
    }
}