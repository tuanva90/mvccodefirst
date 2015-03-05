using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_DI_IOC.Core.NorthWND.Services;
using MVC_DI_IOC.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.IO;
using MVC_DI_IOC.Core.NorthWND.Services.Implement;

namespace MVC_DI_IOC.Web.Controllers
{
    public class CategoryController : Controller
    {
        private UnitOfWork uow = new UnitOfWork();
        //private Repository<Category, CategoryID> cateRepository;
        private readonly ICategoryService _cateservice;

        public CategoryController(ICategoryService cateservice)
        {
            this._cateservice = cateservice;
        }
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View(_cateservice.GetCateList());
        }

        //
        // GET: /Category/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Category/Create

        [HttpPost]
        public ActionResult Create(Category cate)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files["txtpicture"];
                        if (file != null && file.ContentLength > 0)
                        {
                            var input = new BinaryReader(file.InputStream);
                            var filed = input.ReadBytes(file.ContentLength);
                            cate.Picture = filed;
                        }
                    }
                    _cateservice.CreateCate(cate);
                    uow.Save();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View(cate);
            }
        }

        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        public ActionResult Edit(Category cate)
        {
            try
            {
                // TODO: Add update logic here
                if(ModelState.IsValid)
                {
                    _cateservice.UpdateCate(cate);
                    uow.Save();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View(cate);
            }
        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Category/Delete/5

        [HttpPost]
        public ActionResult Delete(int cateID, Category cate)
        {
            try
            {
                // TODO: Add delete logic here
                _cateservice.DeleteCate(cateID);
                uow.Save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
