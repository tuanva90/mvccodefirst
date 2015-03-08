using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UDI.CORE.Entities;
using UDI.CORE.UnitOfWork;
using UDI.CORE.Services;
using System.IO;

namespace DemoMVCEntityFramework.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryService _cate;

        public CategoryController(ICategoryService cate)
        {
            _cate = cate;
        }
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View(_cate.GetAll()); // db.Categories.ToList());
        }

        //
        // GET: /Category/Details/5

        public ActionResult Details(int id = 0)
        {
            Category category = _cate.Get(id); // db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if(Request.Files.Count >0 )
                {
                    var file = Request.Files["txtpicture"];
                    if(file != null && file.ContentLength > 0 )
                    {
                        var red = new BinaryReader(file.InputStream);
                        var filed = red.ReadBytes(file.ContentLength);
                        //var content = Convert.ToBase64String(filed);
                        //var fileName = Path.GetFileName(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~"),fileName);
                        //file.SaveAs(path);
                        category.Picture = filed;
                    }
                }
                _cate.Add(category);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Category category = _cate.Get(id); // db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _cate.Edit(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Category category = _cate.Get(id); // db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = _cate.Get(id); // db.Categories.Find(id);
            _cate.Delete(category);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _cate.Dispose();
            base.Dispose(disposing);
        }
    }
}