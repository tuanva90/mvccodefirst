﻿using System;
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
        public CategoryController(ICategoryService cateservice)
        {
            //this._cateservice = cateservice;
        }
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
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
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}