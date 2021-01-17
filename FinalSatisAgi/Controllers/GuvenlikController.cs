using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class GuvenlikController : Controller
    {
        DbSatisEntities1 db = new DbSatisEntities1();
        // GET: Guvenlik
        public ActionResult GirisYap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GirisYap(USER u)
        {
            if (u != null)
            {
                var bilgiler = db.USER.FirstOrDefault(x => x.user_mail == u.user_mail && x.user_sifre == u.user_sifre);
                if (bilgiler != null)
                {
                    if (bilgiler.user_yetki_id == 1)
                    {
                        Session["LoginYoneticiId"] = bilgiler.user_id;
                        Session["LoginYonetici"] = bilgiler.user_ad;
                        return RedirectToAction("Index", "Yonetici");
                    }
                    else if (bilgiler.user_yetki_id == 2)
                    {
                        Session["LoginFirmaId"] = bilgiler.user_id;
                        Session["LoginFirma"] = bilgiler.user_ad;
                        return RedirectToAction("Index", "Firma");
                    }
                    else
                    {
                        Session["LoginMusteriId"] = bilgiler.user_id;
                        Session["LoginMusteri"] = bilgiler.user_ad;
                        return RedirectToAction("Index", "Musteri");
                    }

                }
                else
                {
                    ViewBag.Error = "Şifre veya E-mail adresini yanlış girdiniz!!!";
                    return View();
                }


            }
            else
            {
                ViewBag.Error = "Şifre veya E-mail adresini yanlış girdiniz!!!";
                return View();
            }
        }
        public ActionResult KayitOl()
        {
            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KayitOl([Bind(Include = "user_id,user_ad,user_soyad,user_mail,user_telefon,user_sifre,user_yetki_id")] USER u)
        {
            if (ModelState.IsValid)
            {
                db.USER.Add(u);
                db.SaveChanges();
                return RedirectToAction("GirisYap", "Guvenlik");
            }

            ViewBag.user_yetki_id = new SelectList(db.YETKI, "yetki_id", "yetki_ad", u.user_yetki_id);
            return View(u);
        }
    }
}