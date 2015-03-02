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
        public ActionResult Index(int? page=1, int? categoryID=0)
        {
            List<Category> lsCate = new List<Category>();
            var all = new Category { CategoryID = 0, CategoryName = "All", Description = "Load all category" };
            lsCate.Add(all);
            lsCate.AddRange(db.Categories.ToList());
            var selectedValue = lsCate.Find(c => c.CategoryID == categoryID);
            ViewBag.CategoryID = new SelectList(lsCate, "CategoryID", "CategoryName", selectedValue);
            ViewBag.SelectedCategoryID = categoryID;
            var products = db.Products;
            pageSize = 3;
            int pageNumber = (page ?? 1);
            
            if (categoryID != 0)
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
                ViewBag.SelectedCategoryID = CategoryID;
                var products = db.Products.Where(p => p.CategoryID == CategoryID).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
            else
            {
                ViewBag.SelectedCategoryID = CategoryID;
                var products = db.Products.Include(p => p.Category).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
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
        //private HttpCookie procook = new HttpCookie();
        
        [HttpPost]
        public ActionResult AddListPro(Product pro)
        {
            if(ModelState.IsValid)
            {
                var ss = Session["Order_" + User.Identity.Name];
                if (ss == null)
                {
                    List<Product> prolist = new List<Product>();
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
                else
                {
                    List<Product> prolist = (List<Product>)ss;
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public int AddToCart(int ? proID = 0, int ? num = 0)
        {
            if (proID > 0 && num > 0)
            {
                var pro = db.Products.Find(proID);
                pro.Quantity = (int)num;
                var ss = Session["Order_" + User.Identity.Name];
                if (ss == null)
                {
                    List<Product> prolist = new List<Product>();
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
                else
                {
                    List<Product> prolist = (List<Product>)ss;
                    prolist.Add(pro);
                    Session["Order_" + User.Identity.Name] = prolist;
                }
                return 1;
            }
            else
            {
                return 0;
            }
            
        }

        // GET: /Product/Details/5
        public ActionResult Details(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return PartialView();
            }
            return PartialView(product);
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