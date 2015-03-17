using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;
using PagedList;
using DemoMVCEntityFramework.Controllers;
using UDI.CORE.Services;

namespace DemoMVCEntityFramework.Controllerss
{
    public class ProductController : BaseController
    {
        private ICategoryService _cate;
        private IProductService _prod;

        private static int pageSize = 3;
        private static int iRetry = 0;

        //public ProductController(IUnitOfWork uow) : base(uow)
        //{
        //}

        public ProductController(ICategoryService cate, IProductService prod)
        {
            _cate = cate;
            _prod = prod;
        }

        //
        // GET: /Product/
        public ActionResult Index(int? page=1, int? categoryID=0)
        {
            List<Category> lsCate = new List<Category>();
            var all = new Category { CategoryID = 0, CategoryName = "All", Description = "Load all category" };
            lsCate.Add(all);
            lsCate.AddRange(_cate.GetAll()); // _uow.Repository<Category>().GetAll().ToList());
            var selectedValue = lsCate.Find(c => c.CategoryID == categoryID);
            ViewBag.CategoryID = new SelectList(lsCate, "CategoryID", "CategoryName", selectedValue);
            ViewBag.SelectedCategoryID = categoryID;
            var products = _prod.GetAll(); // _uow.Repository<Product>().GetAll();
            pageSize = 3;
            int pageNumber = (page ?? 1);
            
            if (categoryID != 0)
            {
                var rsl = products.OrderBy(i => i.ProductID).Where(p => p.CategoryID == categoryID).ToPagedList(pageNumber, pageSize);
                return View(rsl);
            }
            else
            {
                try
                {
                    var rsl = products.OrderBy(i => i.ProductID).ToPagedList(pageNumber, pageSize);
                    iRetry = 0;
                    return View(rsl);
                }
                catch(Exception ex)
                {
                    if (iRetry < 3)
                    {
                        iRetry++;
                        return Index(page, categoryID);
                    }
                    else
                    {
                        iRetry = 0;
                        return View();
                    }                    
                }                
            }
        }
           
        [HttpGet]
        public PartialViewResult LoadProduct(int CategoryID)
        {
            if (CategoryID != 0)
            {
                ViewBag.SelectedCategoryID = CategoryID;
                var products = _prod.GetAll(CategoryID); // _uow.Repository<Product>().GetAll().Where(p => p.CategoryID == CategoryID).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
            else
            {
                ViewBag.SelectedCategoryID = CategoryID;
                //var products = db.Products.Include(p => p.Category).ToList();
                var products = _prod.GetAll(); // _uow.Repository<Product>().GetAll().ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
        }
                
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
                var pro = _prod.Get((int)proID); // _uow.Repository<Product>().Get(p => p.ProductID == proID);
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
            Product product = _prod.Get((int)id); // _uow.Repository<Product>().Get(p => p.ProductID == id);
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
            ViewBag.CategoryID = new SelectList(_cate.GetAll(), "CategoryID", "CategoryName");
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
                _prod.Add(product);
                return RedirectToAction("Create");
            }

            ViewBag.CategoryID = new SelectList(_cate.GetAll(), "CategoryID", "CategoryName", product.CategoryID);
            return Create();
            //return View(product);
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Product product = _prod.Get((int)id); // _uow.Repository<Product>().Get(p => p.ProductID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(_cate.GetAll(), "CategoryID", "CategoryName", product.CategoryID);
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
                _prod.Edit(product);     
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(_cate.GetAll(), "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Product product = _prod.Get((int)id); // _uow.Repository<Product>().Get(p => p.ProductID == id);
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
            Product product = _prod.Get((int)id); // _uow.Repository<Product>().Get(p => p.ProductID == id);
            _prod.Delete(product);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult DeleteProducts(String lsID, int? CategoryID = 0)
        {
            var lsStrID = lsID.Substring(1).Split(',');

            foreach (String item in lsStrID)
            {
                if (item.Trim().Length > 0)
                {
                    var product = _prod.Get(int.Parse(item.Trim()));
                    _prod.Delete(product);
                }
            }

            if (CategoryID != 0)
            {
                ViewBag.SelectedCategoryID = CategoryID;
                var products = _prod.GetAll((int) CategoryID); // _uow.Repository<Product>().GetAll().Where(p => p.CategoryID == CategoryID).ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
            else
            {
                ViewBag.SelectedCategoryID = CategoryID;
                //var products = db.Products.Include(p => p.Category).ToList();
                var products = _prod.GetAll(); // _uow.Repository<Product>().GetAll().ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
        }

        protected override void Dispose(bool disposing)
        {
            _cate.Dispose();
            _prod.Dispose();
            base.Dispose(disposing);
        }

        //public object products { get; set; }
    }
}