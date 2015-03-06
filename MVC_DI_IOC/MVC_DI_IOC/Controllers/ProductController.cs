using MVC_DI_IOC.Core.NorthWND.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_DI_IOC.Web.Controllers
{
    public class ProductController : Controller
    {
        private IUnitOfWork _uow;
        public ProductController(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        //
        // GET: /Product/

        public ActionResult Index()
        {
            var listpro = _uow.Repository<Product, int>().GetAll().ToList();
            return View(listpro);
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            _uow.BeginTransaction();
            var pro = _uow.Repository<Product, int>().Get(id);
            return View(pro);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
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
                _uow.BeginTransaction();
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
    }
}
