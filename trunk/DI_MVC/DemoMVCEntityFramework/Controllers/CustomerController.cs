using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UDI.CORE.Entities;
using UDI.CORE.Services;
using System.Web.Security;

namespace DemoMVCEntityFramework.Controllers
{
    public class CustomerController : Controller
    {
        private ICustomerService _cus;
        private IUserService _usr;

        public CustomerController(ICustomerService cus, IUserService usr)
        {
            _cus = cus;
            _usr = usr;
        }

        //
        // GET: /Customer/
        [Authorize]
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("Login", "Account");
            return View(_cus.GetAll());
        }

        //
        // GET: /Customer/Details/5

        public ActionResult Details(int ? id = 0)
        {
            Customer customer = _cus.Get((int)id); // db.Customers.Find(id);
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
            //var d = (from p in db.Users where p.UserName == User.Identity.Name select p.CustomerID).FirstOrDefault();
            //int id = (from u in db.Users where u.CustomerID == d select u.CustomerID).FirstOrDefault();
            var loginUser = _usr.GetLoginUser(User.Identity.Name);
            var cus = _cus.Find(loginUser.CustomerID); // db.Customers.Find(id);
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
            var loginUser = _usr.GetLoginUser(User.Identity.Name); // db.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            if (loginUser != null)
            {
                customer.CustomerID = loginUser.CustomerID;
            }

            if (ModelState.IsValid)
            {
                _cus.Add(customer);
                //db.Customers.Add(customer);
                //db.SaveChanges();
                return RedirectToAction("Index","Product");
            }

            return View(customer);
        }

        //
        // GET: /Customer/Edit/5

        public ActionResult Edit(int ? id = 0)
        {
            Customer customer = _cus.Get((int)id); // db.Customers.Find(id);
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
                _cus.Edit(customer);
                //db.Entry(customer).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customer/Delete/5

        public ActionResult Delete(int ? id = 0)
        {
            Customer customer = _cus.Get((int)id); // db.Customers.Find(id);
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
            Customer customer = _cus.Get((int)id); // db.Customers.Find(id);
            _cus.Delete(customer);
            //db.Customers.Remove(customer);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _cus.Dispose();
            _usr.Dispose();
            base.Dispose(disposing);
        }
    }
}