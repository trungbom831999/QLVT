using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLVTFinal.Models;

namespace QLVTFinal.Controllers
{
    public class tblAdminsController : Controller
    {
        private QLVatTuEntities db = new QLVatTuEntities();

        // GET: tblAdmins
        public ActionResult Index()
        {
            return View(db.tblAdmins.ToList());
        }

        // GET: tblAdmins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAdmin tblAdmin = db.tblAdmins.Find(id);
            if (tblAdmin == null)
            {
                return HttpNotFound();
            }
            return View(tblAdmin);
        }

        // GET: tblAdmins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Admin_ID,Admin_Avatar,Admin_Username,Admin_Email,Admin_Phone,Admin_NickYahoo,Admin_NickSkype,Roles_ID,Admin_Created,Admin_Log,Admin_LoginType,Admin_Sex,Admin_Birth,Admin_Address,Admin_Permission,Admin_FullName,Admin_Actived,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] tblAdmin tblAdmin)
        {
            if (ModelState.IsValid)
            {
                db.tblAdmins.Add(tblAdmin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblAdmin);
        }

        // GET: tblAdmins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAdmin tblAdmin = db.tblAdmins.Find(id);
            if (tblAdmin == null)
            {
                return HttpNotFound();
            }
            return View(tblAdmin);
        }

        // POST: tblAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Admin_ID,Admin_Avatar,Admin_Username,Admin_Email,Admin_Phone,Admin_NickYahoo,Admin_NickSkype,Roles_ID,Admin_Created,Admin_Log,Admin_LoginType,Admin_Sex,Admin_Birth,Admin_Address,Admin_Permission,Admin_FullName,Admin_Actived,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] tblAdmin tblAdmin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblAdmin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblAdmin);
        }

        // GET: tblAdmins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAdmin tblAdmin = db.tblAdmins.Find(id);
            if (tblAdmin == null)
            {
                return HttpNotFound();
            }
            return View(tblAdmin);
        }

        // POST: tblAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAdmin tblAdmin = db.tblAdmins.Find(id);
            db.tblAdmins.Remove(tblAdmin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
