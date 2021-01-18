using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class FirmaController : Controller
    {
        DbSatisEntities1 db = new DbSatisEntities1();
        // GET: Firma
        public ActionResult Index() // Bu kısım Firma Anasayfa
        {
            return View();
        }
        public ActionResult Urunler() // Ürün listeleme
        {
            var urun = db.URUN.ToList();
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
        public ActionResult YeniUrun()
        {
            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad");
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniUrun([Bind(Include = "urun_id,urun_ad,urun_ebat,urun_hammadde_id,urun_bakim_id,urun_uretici,urun_fiyat,urun_teminat,urun_aciklama,urun_stok")] URUN urun, HttpPostedFileBase UrunResim)
        {
            if (ModelState.IsValid)
            {
                db.URUN.Add(urun);
                db.SaveChanges();

                if (UrunResim != null && UrunResim.ContentLength > 0)
                {
                    string filePath = Path.Combine(Server.MapPath("~/Resim"), urun.urun_id + ".jpg");
                    UrunResim.SaveAs(filePath);
                }
                return RedirectToAction("Urunler");
            }

            ViewBag.urun_bakim_id = new SelectList(db.BAKIM, "bakim_id", "bakim_ad", urun.urun_bakim_id);
            ViewBag.urun_hammadde_id = new SelectList(db.HAMMADDE, "hammadde_id", "hammadde_ad", urun.urun_hammadde_id);
            return View(urun);
        }
        public ActionResult SepetEkle(int? adet, int id)
        {
            if (Session["LoginFirmaId"] != null)
            {
                int userId = (int)Session["LoginFirmaId"];
                SEPET sepettekiUrun = db.SEPET.FirstOrDefault(x => x.sepet_urun_id == id && x.sepet_user_id == userId);
                URUN urun = db.URUN.Find(id);
                if (sepettekiUrun == null)
                {
                    SEPET yeniUrun = new SEPET();
                    yeniUrun.sepet_user_id = userId;
                    yeniUrun.sepet_urun_id = id;
                    yeniUrun.sepet_adet = adet ?? 1;
                    yeniUrun.sepet_tutar = (adet ?? 1) * urun.urun_fiyat;
                    db.SEPET.Add(yeniUrun);
                }
                else
                {
                    sepettekiUrun.sepet_adet = sepettekiUrun.sepet_adet + (adet ?? 1);
                    sepettekiUrun.sepet_tutar = sepettekiUrun.sepet_adet * urun.urun_fiyat;
                }
                db.SaveChanges();
                return RedirectToAction("Urunler", "Firma");
            }
            return RedirectToAction("Urunler", "Firma"); ;
        }
        public ActionResult Sepet()
        {
            int userId = (int)Session["LoginFirmaId"];
            var sepet = db.SEPET.Where(x => x.sepet_user_id == userId);
            return View(sepet.ToList());
        }
        public ActionResult SepetGuncelle(int? adet, int id)
        {

            SEPET sepet = db.SEPET.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            URUN urun = db.URUN.Find(sepet.sepet_urun_id);

            sepet.sepet_adet = adet ?? 1;
            sepet.sepet_tutar = sepet.sepet_adet * urun.urun_fiyat;
            db.SaveChanges();
            return RedirectToAction("Sepet");
        }
        public ActionResult SepetSil(int id)
        {
            SEPET sepet = db.SEPET.Find(id);
            db.SEPET.Remove(sepet);
            db.SaveChanges();
            return RedirectToAction("Sepet");
        }
        public ActionResult SiparisDetay(int id)
        {
            var siparisDetay = db.SIPARIS_K.Where(a => a.siparis_k_id == id).ToList();
            return View(siparisDetay.ToList());
        }
        public ActionResult SiparisTamamla()
        {
            int userID = (int)Session["LoginFirmaId"];
            SIPARIS siparis = new SIPARIS()
            {
                siparis_ad = Request.Form.Get("siparis_ad"),
                siparis_soyad = Request.Form.Get("siparis_soyad"),
                siparis_adres = Request.Form.Get("siparis_adres"),
                siparis_tarih = DateTime.Now,
                siparis_tc= Request.Form.Get("siparis_tc"),
                siparis_telefon = Request.Form.Get("siparis_telefon"),
                siparis_user_id = userID
            };

            IEnumerable<SEPET> sepettekiUrunler = db.SEPET.Where(a => a.sepet_user_id == userID).ToList();

            foreach (SEPET sepetUrunu in sepettekiUrunler)
            {
                SIPARIS_K yeniKalem = new SIPARIS_K()
                {
                    siparis_adet = sepetUrunu.sepet_adet,
                    siparis_tutar = sepetUrunu.sepet_tutar,
                    siparis_k_urun_id = sepetUrunu.sepet_urun_id
                };

                siparis.SIPARIS_K.Add(yeniKalem);

                db.SEPET.Remove(sepetUrunu);
            }

            db.SIPARIS.Add(siparis);
            db.SaveChanges();

            return View();

        }
        public ActionResult Tamamlandi()
        { 
            return View();
        }
        public ActionResult Siparislerim()
        {
            var userId = (int)Session["LoginFirmaiId"];
            var siparisler = db.SIPARIS.Where(x => x.siparis_user_id == userId).ToList();
            return View(siparisler);
        }
        public ActionResult SiparisListele()
        {
            var siparis = db.SIPARIS.ToList();
            return View(siparis);
        }

    }
}