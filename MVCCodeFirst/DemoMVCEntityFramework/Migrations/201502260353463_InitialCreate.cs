namespace DemoMVCEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 15),
                        Description = c.String(nullable: false, maxLength: 500),
                        Picture = c.Binary(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 50),
                        CategoryID = c.Int(nullable: false),
                        QuantityPerUnit = c.String(nullable: false, maxLength: 50),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitsInStock = c.Int(nullable: false),
                        UnitsOnOrder = c.Int(nullable: false),
                        ReorderLevel = c.Int(nullable: false),
                        Discontinued = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Category", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.OrderDetail",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quanlity = c.Int(nullable: false),
                        Discount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderID, t.ProductID })
                .ForeignKey("dbo.Order", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(nullable: false, maxLength: 50),
                        OrderDate = c.DateTime(nullable: false),
                        RequiredDate = c.DateTime(nullable: false),
                        ShippedDate = c.DateTime(nullable: false),
                        ShipVia = c.Int(nullable: false),
                        Freight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShipName = c.String(nullable: false, maxLength: 50),
                        ShipAddress = c.String(nullable: false, maxLength: 50),
                        ShipCity = c.String(nullable: false, maxLength: 50),
                        ShipRegion = c.String(nullable: false, maxLength: 50),
                        ShipPostalCode = c.String(nullable: false, maxLength: 50),
                        ShipCountry = c.String(nullable: false, maxLength: 50),
                        Customer_CustomerID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Customer", t => t.Customer_CustomerID)
                .Index(t => t.Customer_CustomerID);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        CustomerID = c.Int(nullable: false),
                        CompanyName = c.String(nullable: true, maxLength: 50),
                        ContactName = c.String(nullable: true, maxLength: 50),
                        ContactTitle = c.String(nullable: true, maxLength: 50),
                        Address = c.String(nullable: true, maxLength: 60),
                        City = c.String(nullable: true, maxLength: 50),
                        Region = c.String(nullable: true, maxLength: 50),
                        PostalCode = c.String(nullable: true, maxLength: 50),
                        Country = c.String(nullable: true, maxLength: 50),
                        Phone = c.String(nullable: true, maxLength: 20),
                        Fax = c.String(nullable: true, maxLength: 50),
                    })
                .PrimaryKey(t => t.CustomerID)
                .ForeignKey("dbo.User", t => t.CustomerID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(nullable: false),
                        Bool = c.Boolean(nullable: false),
                        Roles = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Customer", new[] { "CustomerID" });
            DropIndex("dbo.Order", new[] { "Customer_CustomerID" });
            DropIndex("dbo.OrderDetail", new[] { "ProductID" });
            DropIndex("dbo.OrderDetail", new[] { "OrderID" });
            DropIndex("dbo.Product", new[] { "CategoryID" });
            DropForeignKey("dbo.Customer", "CustomerID", "dbo.User");
            DropForeignKey("dbo.Order", "Customer_CustomerID", "dbo.Customer");
            DropForeignKey("dbo.OrderDetail", "ProductID", "dbo.Product");
            DropForeignKey("dbo.OrderDetail", "OrderID", "dbo.Order");
            DropForeignKey("dbo.Product", "CategoryID", "dbo.Category");
            DropTable("dbo.User");
            DropTable("dbo.Customer");
            DropTable("dbo.Order");
            DropTable("dbo.OrderDetail");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
        }
    }
}
