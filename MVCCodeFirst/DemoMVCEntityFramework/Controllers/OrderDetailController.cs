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
    public class OrderDetailController : Controller
    {
        private NorthWNDContext db = new NorthWNDContext();

        //
        // GET: /OrderDetail/

        public ActionResult Index()
        {
            var orderdetails = db.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            return View(orderdetails.ToList());
        }

        //
        // GET: /OrderDetail/Details/5

        public ActionResult Details(int id = 0)
        {
            OrderDetail orderdetail = db.OrderDetails.Find(id);
            if (orderdetail == null)
            {
                return HttpNotFound();
            }
            return View(orderdetail);
        }

        //
        // GET: /OrderDetail/Create

        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        //
        // POST: /OrderDetail/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderDetail orderdetail)
        {
            if (ModelState.IsValid)
            {

                db.OrderDetails.Add(orderdetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", orderdetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderdetail.ProductID);
            return View(orderdetail);
        }

        //
        // GET: /OrderDetail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OrderDetail orderdetail = db.OrderDetails.Find(id);
            if (orderdetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", orderdetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderdetail.ProductID);
            return View(orderdetail);
        }

        //
        // POST: /OrderDetail/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderDetail orderdetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderdetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", orderdetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderdetail.ProductID);
            return View(orderdetail);
        }

        //
        // GET: /OrderDetail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OrderDetail orderdetail = db.OrderDetails.Find(id);
            if (orderdetail == null)
            {
                return HttpNotFound();
            }
            return View(orderdetail);
        }

        //
        // POST: /OrderDetail/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderdetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderdetail);
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