using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class HomeController : Controller
    {
        DbSatisEntities1 db = new DbSatisEntities1();
        // GET: Home
        public ActionResult Hakkında()
        {
            return View();
        }
        public ActionResult Index()
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

    }
}