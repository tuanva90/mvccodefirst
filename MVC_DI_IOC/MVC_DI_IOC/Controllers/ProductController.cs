using MVC_DI_IOC.Core.NorthWND.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Linq.Expressions;
using MVC_DI_IOC.Data;


namespace MVC_DI_IOC.Web.Controllers
{
    public class ProductController : Controller
    {
        public NorthWNDContext context = new NorthWNDContext();
        private IUnitOfWork _uow;
        private static int pageSize = 3;
        private static int iRetry = 0;
        public ProductController(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        //
        // GET: /Product/

        public ActionResult Index(int? page=1, int? cateid = 0)
        {
            List<Category> lscate = new List<Category>();
            //List<Product> lspro = new List<Product>();
            var all = new Category { ID = 0, CategoryName = "All", Description = "Load all category" };
            lscate.Add(all);
            _uow.BeginTransaction();
            var cate = _uow.Repository<Category, int>().GetAll().ToList();
            lscate.AddRange(cate);
            var selectedValue = lscate.Find(c => c.ID == cateid);
            ViewBag.CategoryID = new SelectList(lscate, "ID", "CategoryName", selectedValue);
            ViewBag.SelectedCategoryID = cateid;
            pageSize = 3;
            int pageNumber = (page ?? 1);
            if (cateid != 0)
            {
                var products = _uow.Repository<Product, int>().GetAll();
                var rsl = products.OrderBy(i => i.ID).Where(p => p.CategoryID == cateid).ToPagedList(pageNumber, pageSize);
                return View(rsl);
            }
            else
            {
                try
                {
                    var rsl = _uow.Repository<Product, int>().GetAll().ToPagedList(pageNumber, pageSize);
                    iRetry = 0;
                    return View(rsl);
                }
                catch (Exception ex)
                {
                    if (iRetry < 3)
                    {
                        iRetry++;
                        return Index(page, cateid);
                    }
                    else
                    {
                        iRetry = 0;
                        return View();
                    }
                }
            }
            Dispose();
        }
        [HttpGet]
        public PartialViewResult LoadProduct(int CategoryID)
        {
            if (CategoryID != 0)
            {
                ViewBag.SelectedCategoryID = CategoryID;
                _uow.BeginTransaction();
                var products = _uow.Repository<Product, int>().GetAll();
                var rsl = products.OrderBy(i => i.ID).Where(p => p.CategoryID == CategoryID).ToList();
                return PartialView("tableView", rsl.ToPagedList(1, pageSize));
            }
            else
            {
                ViewBag.SelectedCategoryID = CategoryID;
                var products = _uow.Repository<Product, int>().GetAll().ToList();
                return PartialView("tableView", products.ToPagedList(1, pageSize));
            }
        }

        [HttpGet]
        public int AddToCart(int? proID = 0, int? num = 0)
        {
            if (proID > 0 && num > 0)
            {
                _uow.BeginTransaction();
                var pro = _uow.Repository<Product, int>().Get((int)proID);
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
        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            _uow.BeginTransaction();
            var pro = _uow.Repository<Product, int>().Get(id);
            Dispose();
            return View(pro);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            _uow.BeginTransaction();
            var cate = _uow.Repository<Category, int>().GetAll().ToList();
            ViewBag.CategoryID = new SelectList(cate, "ID", "CategoryName");
            Dispose();
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(Product pro)
        {
            try
            {
                // TODO: Add insert logic here
                //_uow.BeginTransaction();
                _uow.Repository<Product, int>().Insert(pro);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id)
        {
            _uow.BeginTransaction();
            var pro = _uow.Repository<Product, int>().Get(id);
            return View(pro);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(Product pro)
        {
            try
            {
                // TODO: Add update logic here
                _uow.BeginTransaction();
                _uow.Repository<Product, int>().Update(pro);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            _uow.BeginTransaction();
            var pro = _uow.Repository<Product, int>().Get(id);
            return View(pro);
        }

        //
        // POST: /Product/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Product pro)
        {
            try
            {
                // TODO: Add delete logic here
                _uow.BeginTransaction();
                _uow.Repository<Product, int>().Delete(id);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
