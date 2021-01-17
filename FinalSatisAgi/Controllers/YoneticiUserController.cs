using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class YoneticiUserController : Controller
    {
        private DbSatisEntities1 db = new DbSatisEntities1();

        public ActionResult Index()
        {
            var uSER = db.USER.Include(u => u.YETKI);
            return View(uSER.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USER.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        public ActionResult Create()
        {
            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,user_ad,user_soyad,user_mail,user_telefon,user_sifre,user_yetki_id,user_adres")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.USER.Add(uSER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad", uSER.user_yetki_id);
            return View(uSER);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USER.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad", uSER.user_yetki_id);
            return View(uSER);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_ad,user_soyad,user_mail,user_telefon,user_sifre,user_yetki_id,user_adres")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad", uSER.user_yetki_id);
            return View(uSER);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USER.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USER uSER = db.USER.Find(id);
            db.USER.Remove(uSER);
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
        public ActionResult Firma()
        {
            var firmalar = db.USER.Where(x=> x.user_yetki_id == 2).ToList();
            return View(firmalar);
        }
        public ActionResult Musteri()
        {
            var firmalar = db.USER.Where(x => x.user_yetki_id == 3).ToList();
            return View(firmalar);
        }
        public ActionResult Yonetici()
        {
            var firmalar = db.USER.Where(x => x.user_yetki_id == 1).ToList();
            return View(firmalar);
        }
    }
}
