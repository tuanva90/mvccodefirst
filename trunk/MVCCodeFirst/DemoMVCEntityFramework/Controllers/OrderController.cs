using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMVCEntityFramework.Models;
using DemoMVCEntityFramework.Data_Access_Layer;

namespace DemoMVCEntityFramework.Controllers
{
    public class OrderController : Controller
    {
        private NorthWNDContext db = new NorthWNDContext();

        //
        // GET: /Order/

        public ActionResult Index()
        {
            var curOrder = (List<Product>) Session["Order_" + User.Identity.Name];
            if (curOrder == null)
                curOrder = new List<Product>();
            ViewData["CurOrder"] = curOrder;
            var orders = db.Orders.Include(o => o.Customer);
            return View(orders.ToList());
        }

        
        public ActionResult AddOrder()
        {
            var d = (from p in db.Users where p.UserName == User.Identity.Name select p.CustomerID).FirstOrDefault();
            int id = (from u in db.Users where u.CustomerID == d select u.CustomerID).FirstOrDefault();

            var cus = db.Customers.Find(id);
            var curOrder = (List<Product>)Session["Order_" + User.Identity.Name];
            if (curOrder != null)
            {
                db.Orders.Add(new Order { Customer = cus, ShipVia = 1, Freight = 1, OrderDate = DateTime.Today, RequiredDate = DateTime.Today, ShippedDate = DateTime.Today, CustomerID = cus.CustomerID });
                db.SaveChanges(); ;
                var ord1 = db.Orders.Where(c => c.CustomerID == cus.CustomerID).Max(c => c.OrderID);
                var ord = db.Orders.Where(c => c.OrderID == ord1).First();
                
                foreach (Product item in curOrder)
                {
                    db.OrderDetails.Add(new OrderDetail { Order = ord, Product = item, Quanlity = item.Quantity, OrderID = ord.OrderID, ProductID = item.ProductID});
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // GET: /Order/Create

        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            return View();
        }

        //
        // POST: /Order/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            return View(order);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", order.CustomerID);
            return View(order);
        }

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}