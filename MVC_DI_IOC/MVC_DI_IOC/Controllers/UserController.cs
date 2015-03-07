using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_DI_IOC.Data;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using MVC_DI_IOC.Core;
using System.IO;
using MVC_DI_IOC.Core.NorthWND.Data;
using System.Web.Security;

namespace MVC_DI_IOC.Web.Controllers
{
    public class UserController : Controller
    {
        private IUnitOfWork _uow;
        public UserController(IUnitOfWork uow)
        {
            //this._userservice = userservice;
            _uow = uow;
        }
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(_uow.Repository<User, int>().GetAll().ToList());
        }

        //Get:User

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //
        // POST: /User/Details/5
        [HttpPost]
        public ActionResult Login(User us)
        {
            
            _uow.BeginTransaction();
            var user = _uow.Repository<User, int>().Get(us.ID);
            if(user!=null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, user.Remember);
                return RedirectToAction("Login","User");
            }
            else
            {
                ModelState.AddModelError("", "Login data is incorrect!");
            }
            return View(user);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }

        //
        // GET: /User/Create

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                // TODO: Add insert logic here
                _uow.BeginTransaction();
                _uow.Repository<User, int>().Insert(user);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(user);
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            _uow.BeginTransaction();
            var user = _uow.Repository<User, int>().Get(id);
            //var user = _uow.Commit();
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            try
            {
                // TODO: Add update logic here
                _uow.BeginTransaction();
                _uow.Repository<User, int>().Update(user);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(user);
            }
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            _uow.BeginTransaction();
            var user = _uow.Repository<User, int>().Get(id);
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost]
        public ActionResult Delete(int userID, User user)
        {
            try
            {
                // TODO: Add delete logic here
                _uow.BeginTransaction();
                _uow.Repository<User, int>().Delete(userID);
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
            _uow.BeginTransaction();
            _uow.Dispose();
        }

    }
}
