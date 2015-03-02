using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMVCEntityFramework.Models;
using DemoMVCEntityFramework.Data_Access_Layer;
using System.Web.Security;

namespace DemoMVCEntityFramework.Controllers
{
    public class CustomerController : Controller
    {
        private NorthWNDContext db = new NorthWNDContext();

        //
        // GET: /Customer/
        [Authorize]
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("Login", "Account");
            return View(db.Customers.ToList());
        }

        //
        // GET: /Customer/Details/5

        public ActionResult Details(int ? id = 0)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // GET: /Customer/Create
        [HttpGet]
        public ActionResult Create()
        {
            var d = (from p in db.Users where p.UserName == User.Identity.Name select p.CustomerID).FirstOrDefault();
            int id = (from u in db.Users where u.CustomerID == d select u.CustomerID).FirstOrDefault();

            var cus = db.Customers.Find(id);
            if (cus == null)
                return View();
            else
                return Content("<a>This user already created customer!</a>");
        }

        //
        // POST: /Customer/Create
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            var s = db.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            if (s != null)
            {
                customer.CustomerID = s.CustomerID;
            }



            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index","Product");
            }

            return View(customer);
        }

        //
        // GET: /Customer/Edit/5

        public ActionResult Edit(int ? id = 0)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customer/Delete/5

        public ActionResult Delete(int ? id = 0)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ? id = 0)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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