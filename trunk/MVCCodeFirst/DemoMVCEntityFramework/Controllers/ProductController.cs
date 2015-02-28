using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMVCEntityFramework.Models;
using DemoMVCEntityFramework.Data_Access_Layer;
using PagedList;

namespace DemoMVCEntityFramework.Controllerss
{
    public class ProductController : Controller
    {
        private NorthWNDContext db = new NorthWNDContext();
        private static int pageSize = 3;

        //
        // GET: /Product/
        public ActionResult Index(int? page, int? categoryID)
        {
            //SelectListItem all = new SelectListItem() { Value="0",Text="All" };
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            var products = db.Products.Include(p => p.Category);
            //db.Categories.(all);
            pageSize = 3;
            int pageNumber = (page ?? 1);
            
            if (categoryID != null)
            {
                var rsl = products.OrderBy(i => i.ProductID).Where(p => p.CategoryID == categoryID).ToPagedList(pageNumber, pageSize);
                return View(rsl);
            }
            else
            {
                var rsl = products.OrderBy(i => i.ProductID).ToPagedList(pageNumber, pageSize);
                return View(rsl);
            }
           
        }
        [HttpGet]
        public PartialViewResult LoadProduct(int CategoryID)
        {
            if (CategoryID != 0)
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                ViewBag.SelectedCategoryID = CategoryID;
                var products = db.Products.Where(p => p.CategoryID == CategoryID).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
            else
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                ViewBag.SelectedCategoryID = CategoryID;
                var products = db.Products.Include(p => p.Category).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
            //Product pro = new Product();
            //return Json(products, JsonRequestBehavior.AllowGet);

        }
        //
        [NonAction]
        [HttpGet]
        public bool chkOrder(int id)
        {
            var order = db.Orders.Where(o => o.OrderID == id).FirstOrDefault();
            if(order == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public ActionResult AddOrder(Product pro)
        {
            var product = db.Products.Where(p => p.ProductID == pro.ProductID).FirstOrDefault();
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var customer = db.Customers.Where(c => c.CustomerID == user.CustomerID).FirstOrDefault();
            Order or = new Order();
            OrderDetail odt = new OrderDetail();
            or.OrderID = 1;
            or.CustomerID = user.CustomerID;
            or.OrderDate = DateTime.Today;
            or.RequiredDate = DateTime.Today;
            or.ShippedDate = DateTime.Today;
            or.ShipName = customer.ContactName;
            or.ShipAddress = customer.Address;
            or.ShipCity = customer.City;
            or.ShipCountry = customer.Country;
            or.ShipPostalCode = customer.PostalCode;
            or.ShipRegion = customer.Region;
            or.ShipVia = 0;
            or.Freight = 0;
            var exist = chkOrder(or.OrderID);
            if (exist == true)
            {
                odt.OrderID = or.OrderID;
                odt.ProductID = product.ProductID;
                odt.Quanlity = pro.Quantity;
                odt.UnitPrice = product.UnitPrice;
                odt.Discount = 0;
                //if (ModelState.IsValid)
                //{
                    db.Orders.Add(or);
                    db.OrderDetails.Add(odt);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
            }
            else
            {
                odt.OrderID = or.OrderID;
                odt.ProductID = product.ProductID;
                odt.Quanlity = pro.Quantity;
                odt.UnitPrice = product.UnitPrice;
                odt.Discount = 0;
                //if (ModelState.IsValid)
                //{
                    db.OrderDetails.Add(odt);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
            }

            //return Index(1,1);
        }


        //[HttpPost]
        //public ActionResult AddOrder(int ProductID=0, int quantity=0)
        //{
        //    var product = db.Products.Where(p => p.ProductID == ProductID).FirstOrDefault();
        //    var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
        //    var customer = db.Customers.Where(c => c.CustomerID == user.CustomerID).FirstOrDefault();
        //    //CustomerID = user.CustomerID;
        //    Order or = new Order();
        //    OrderDetail odt = new OrderDetail();
        //    or.CustomerID = user.CustomerID;
        //    or.OrderDate = DateTime.Today;
        //    or.RequiredDate = DateTime.Today;
        //    or.ShippedDate = DateTime.Today;
        //    or.ShipName = customer.ContactName;
        //    or.ShipAddress = customer.Address;
        //    or.ShipCity = customer.City;
        //    or.ShipCountry = customer.Country;
        //    or.ShipPostalCode = customer.PostalCode;
        //    or.ShipRegion = customer.Region;
        //    or.ShipVia = 0;
        //    or.Freight = 0;
        //    var exist = chkOrder(or.OrderID);
        //    if(exist == true)
        //    {
        //        odt.OrderID = or.OrderID;
        //        odt.ProductID = product.ProductID;
        //        odt.Quanlity = quantity;
        //        odt.UnitPrice = product.UnitPrice;
        //        odt.Discount = 0;
        //        if(ModelState.IsValid)
        //        {
        //            db.Orders.Add(or);
        //            db.OrderDetails.Add(odt);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    else
        //    {
        //        odt.OrderID = or.OrderID;
        //        odt.ProductID = product.ProductID;
        //        odt.Quanlity = quantity;
        //        odt.UnitPrice = product.UnitPrice;
        //        odt.Discount = 0;
        //        if(ModelState.IsValid)
        //        {
        //            db.OrderDetails.Add(odt);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }

        //    return View(product);
        //}
        // GET: /Product/Details/5
        public ActionResult Details(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return Create();
            //return View(product);
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // POST: /Product/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //public object products { get; set; }
    }
}