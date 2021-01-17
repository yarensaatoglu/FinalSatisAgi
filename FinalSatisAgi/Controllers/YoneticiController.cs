using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class YoneticiController : Controller
    {
        DbSatisEntities1 db = new DbSatisEntities1();
        // GET: Yonetici
        public ActionResult Index()
        {
            var sumAdet = db.SIPARIS_K.Sum(x => x.siparis_adet);
            var sumToplamFiyat = db.SIPARIS_K.Sum(x => x.siparis_tutar);
            return View();
        }
        public ActionResult Urunler()
        {
            var urun = db.URUN.ToList();
            return View(urun);
        }
        public ActionResult YeniUrun()
        {
            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad");
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniUrun([Bind(Include = "urun_id,urun_ad,urun_ebat,urun_hammadde_id,urun_bakim_id,urun_uretici,urun_fiyat,urun_teminat,urun_kisa_aciklama, urun_uzun_aciklama,urun_resim,urun_stok")] URUN urun)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    string dosyaAdi = Path.GetFileName(Request.Files[0].FileName);
                    string uzanti = Path.GetExtension(Request.Files[0].FileName);
                    var yol = "~/Resim/" + dosyaAdi + uzanti;
                    Request.Files[0].SaveAs(Server.MapPath(yol));
                    urun.urun_resim = "/Resim/" + dosyaAdi + uzanti;
                }
                db.URUN.Add(urun);
                db.SaveChanges();
                return RedirectToAction("Urunler");
            }

            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad", urun.urun_bakim_id);
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad", urun.urun_hammadde_id);
            return View(urun);
        }
        public ActionResult UrunDetay(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URUN urun = db.URUN.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }
        public ActionResult UrunDuzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URUN urun = db.URUN.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad", urun.urun_bakim_id);
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad", urun.urun_hammadde_id);
            return View(urun);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UrunDuzenle([Bind(Include = "urun_id,urun_ad,urun_ebat,urun_hammadde_id,urun_bakim_id,urun_uretici,urun_fiyat,urun_teminat,urun_uzun_aciklama,urun_kisa_aciklama,urun_stok")] URUN urun, HttpPostedFileBase UrunResim)
        {
            if (ModelState.IsValid)
            {
                db.Entry(urun).State = EntityState.Modified;
                db.SaveChanges();
                if (UrunResim != null && UrunResim.ContentLength > 0)
                {
                    string filePath = Path.Combine(Server.MapPath("~/Resim"), urun.urun_id + ".jpg");
                    UrunResim.SaveAs(filePath);
                }
                return RedirectToAction("Index");
            }
            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad", urun.urun_bakim_id);
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad", urun.urun_hammadde_id);
            return View(urun);
        }
        public ActionResult UrunSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URUN urun = db.URUN.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // POST: FirmaUrun/Delete/5
        [HttpPost, ActionName("UrunSil")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            URUN urun = db.URUN.Find(id);
            db.URUN.Remove(urun);
            db.SaveChanges();

            string filePath = Path.Combine(Server.MapPath("~/Resim"), urun.urun_id + ".jpg");
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                fi.Delete();
            }

            return RedirectToAction("Urunler");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Hammadde()
        {
            return View(db.HAMMADDE.ToList());
        }
        

        // GET: hammadde/Create
        public ActionResult YeniHammadde()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniHammadde([Bind(Include = "hammadde_id,hammadde_ad,hammadde_stok")] HAMMADDE hammadde)
        {
            if (ModelState.IsValid)
            {
                db.HAMMADDE.Add(hammadde);
                db.SaveChanges();
                return RedirectToAction("Hammadde");
            }

            return View(hammadde);
        }

        public ActionResult HammaddeDuzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HAMMADDE hammadde = db.HAMMADDE.Find(id);
            if (hammadde == null)
            {
                return HttpNotFound();
            }
            return View(hammadde);
        }
        [ValidateAntiForgeryToken]
        public ActionResult HammaddeDuzenle([Bind(Include = "hammadde_id,hammadde_ad,hammadde_stok")] HAMMADDE hammadde)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hammadde).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Hammadde");
            }
            return View(hammadde);
        }

        public ActionResult HammaddeSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HAMMADDE hammadde = db.HAMMADDE.Find(id);
            if (hammadde == null)
            {
                return HttpNotFound();
            }
            return View(hammadde);
        }

        [HttpPost, ActionName("HammaddeSil")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedH(int id)
        {
            HAMMADDE hammadde = db.HAMMADDE.Find(id);
            db.HAMMADDE.Remove(hammadde);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SiparisListele()
        {
            var siparis = db.SIPARIS.ToList();
            return View(siparis);
        }

    }
}