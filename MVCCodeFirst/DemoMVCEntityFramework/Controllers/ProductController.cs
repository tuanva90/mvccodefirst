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

namespace DemoMVCEntityFramework.Controllers
{
    public class ProductController : Controller
    {
        private NorthWNDContext db = new NorthWNDContext();
        private static int pageSize = 3;

        //
        // GET: /Product/
        public ActionResult Index(int? page, int? categoryID)
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            var products = db.Products.Include(p => p.Category);
            
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
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SelectedCategoryID = CategoryID;
            var products = db.Products.Where(p => p.CategoryID == CategoryID).ToList();
            return PartialView("tableView", products.ToPagedList(1, pageSize));
            //Product pro = new Product();
            //return Json(products, JsonRequestBehavior.AllowGet);

        }
        //
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
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
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