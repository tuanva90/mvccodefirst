using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;
using UDI.CORE.Services;

namespace DemoMVCEntityFramework.Controllers
{
    public class OrderController : Controller
    {
        private IOrderService _ord;
        private IUserService _usr;
        private ICustomerService _cus;

        public OrderController(IOrderService ord, IUserService usr, ICustomerService cus)
        {
            _ord = ord;
            _usr = usr;
            _cus = cus;
        }

        //
        // GET: /Order/
        [Authorize]
        public ActionResult Index()
        {
            var curOrder = (List<Product>) Session["Order_" + User.Identity.Name];
            if (curOrder == null)
                curOrder = new List<Product>();
            ViewData["CurOrder"] = curOrder;
            var user = _usr.GetLoginUser(User.Identity.Name); // db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                var orders = _ord.GetListOrder(user.CustomerID); // db.Orders.Where(o => o.CustomerID == user.CustomerID).ToList();
                return View(orders);
            }
            else
            {
                //WebSecurity.Logout();
                FormsAuthentication.SignOut();
                //return RedirectToAction("Index", "Home");
                return Redirect("~/");
            }
        }

        [Authorize]
        public ActionResult AddOrder()
        {
            //var d = (from p in db.Users where p.UserName == User.Identity.Name select p.CustomerID).FirstOrDefault();
            //int id = (from u in db.Users where u.CustomerID == d select u.CustomerID).FirstOrDefault();
            var userlogin = _usr.GetLoginUser(User.Identity.Name);
            var cus = _cus.Get(userlogin.CustomerID); // db.Customers.Find(id);
            var curOrder = (List<Product>)Session["Order_" + User.Identity.Name];
            if (curOrder != null & cus != null)
            {
                _ord.AddOrder(cus, curOrder);
                Session["Order_" + User.Identity.Name] = null;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index");
        }
        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = _ord.Get(id); // db.Orders.Find(id);
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
            ViewBag.CustomerID = new SelectList(_cus.GetAll(), "CustomerID", "CompanyName");
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
                _ord.Add(order);
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(_cus.GetAll(), "CustomerID", "CompanyName", order.CustomerID);
            return View(order);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Order order = _ord.Get(id); // db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(_ord.GetAll(), "CustomerID", "CompanyName", order.CustomerID);
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
                _ord.Edit(order);
                //db.Entry(order).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(_cus.GetAll(), "CustomerID", "CompanyName", order.CustomerID);
            return View(order);
        }

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Order order = _ord.Get(id); // db.Orders.Find(id);
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
            Order order = _ord.Get(id); // db.Orders.Find(id);
            _ord.Delete(order);
            //db.Orders.Remove(order);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _cus.Dispose();
            _ord.Dispose();
            _usr.Dispose();
            base.Dispose(disposing);
        }
    }
}