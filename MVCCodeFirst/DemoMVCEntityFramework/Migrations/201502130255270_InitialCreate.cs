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
                "dbo.User",
                c => new
                {
                    UserID= c.Int(nullable: false, identity:true),
                    UserName= c.String(nullable:false,maxLength:50),
                    UserPassword=c.String(nullable:false,maxLength:20),
                    Role=c.String(nullable:false,maxLength:10),
                }
                )
                .PrimaryKey(t=>t.UserID);

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
                        CustomerID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 50),
                        ContactName = c.String(nullable: false, maxLength: 50),
                        ContactTitle = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 60),
                        City = c.String(nullable: false, maxLength: 50),
                        Region = c.String(nullable: false, maxLength: 50),
                        PostalCode = c.String(nullable: false, maxLength: 50),
                        Country = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Fax = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Order", new[] { "Customer_CustomerID" });
            DropIndex("dbo.OrderDetail", new[] { "ProductID" });
            DropIndex("dbo.OrderDetail", new[] { "OrderID" });
            DropIndex("dbo.Product", new[] { "CategoryID" });
            DropForeignKey("dbo.Order", "Customer_CustomerID", "dbo.Customer");
            DropForeignKey("dbo.OrderDetail", "ProductID", "dbo.Product");
            DropForeignKey("dbo.OrderDetail", "OrderID", "dbo.Order");
            DropForeignKey("dbo.Product", "CategoryID", "dbo.Category");
            DropTable("dbo.Customer");
            DropTable("dbo.Order");
            DropTable("dbo.OrderDetail");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
        }
    }
}