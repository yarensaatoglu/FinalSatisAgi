using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalSatisAgi.Models.Entity;

namespace FinalSatisAgi.Controllers
{
    public class MusteriController : Controller
    {
        DbSatisEntities1 db = new DbSatisEntities1(); // Veritabanı nesnesi oluşturulur.
        // GET: Musteri
        public ActionResult Index() //Müşteri anasayfadır, ürünler listelenir
        {
            var urun = db.URUN.ToList();
            return View(urun);
        }
        public ActionResult UrunDetaylar(int? id) // Ürün hakkında uzun açıklamalar yer alır
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
        public ActionResult Profil() //Müşteri kendi profil bilgilerini görür
        {
            //Güvenlik Controller'ında girişte Session ile kullanıcı bilgisi tutulur burada da o bilgi kullanılır
            int userId = (int)Session["LoginMusteriId"];
            USER musteri = db.USER.Find(userId);
            if (musteri == null)
            {
                return HttpNotFound();
            }
            return View(musteri);
        }
        public ActionResult Sepet() //Yine Session ile llogin yapmış Müşterinin sepeti gösterilir.
        {
            int userId = (int)Session["LoginMusteriId"];
            var sepet = db.SEPET.Where(x => x.sepet_user_id== userId);
            return View(sepet.ToList());
        }
        public ActionResult SepetEkle(int? adet, int id) //Sepete ürün ekleme işlemi yapılır
        {
            if (Session["LoginMusteriId"] != null)
            {
                int userId = (int)Session["LoginMusteriId"];
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
                return RedirectToAction("Index", "Musteri");
            }
            return RedirectToAction("Index", "Musteri"); ;
        }
        public ActionResult SepetGuncelle(int? adet, int id) // Sepet güncellede ürün sayısı güncellenir
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
        public ActionResult SiparisTamamla()// Siparis tamamlanır 
        {
            int userID = (int)Session["LoginMusteriId"];
            SIPARIS siparis = new SIPARIS()
            {
                siparis_ad = Request.Form.Get("siparis_ad"), // burada View tarfında girilen bilgiler çekilir
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
            //Bu kısım 3-d ödeme için yazılmıştır fakat banka bilgisi vs. olmadığı için aktif değildir.
            /*
            string ClientId = "100300000"; // Bankadan aldığınız mağaza kodu
            string Amount = sepetUrunleri.Sum(a => a.sepet_tutar).ToString(); // sepettteki ürünlerin toplam fiyatı
            string Oid = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now); // sipariş id oluşturuyoruz. her sipariş için farklı olmak zorunda
            string OnayURL = "http://localhost:44388/Siparis/Tamamlandi"; // Ödeme tamamlandığında bankadan verilerin geleceği url
            string HataURL = "http://localhost:44388/Siparis/Hatali"; // Ödeme hata verdiğinde bankadan gelen verilerin gideceği url
            string RDN = "asdf"; // hash karşılaştırması için eklenen rast gele dizedir
            string StoreKey = "123456"; // Güvenlik anahtarı bankanın sanal pos sayfasından alıyoruz


            string TransActionType = "Auth"; // bu bölüm sabit değişmiyor
            string Instalment = "";
            string HashStr = ClientId + Oid + Amount + OnayURL + HataURL + TransActionType + Instalment + RDN + StoreKey; // Hash oluşturmak için bankanın bizden istediği stringleri birleştiriyoruz

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] HashBytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(HashStr);
            byte[] InputBytes = sha.ComputeHash(HashBytes);
            string Hash = Convert.ToBase64String(InputBytes);

            ViewBag.ClientId = ClientId;
            ViewBag.Oid = Oid;
            ViewBag.okUrl = OnayURL;
            ViewBag.failUrl = HataURL;
            ViewBag.TransActionType = TransActionType;
            ViewBag.RDN = RDN;
            ViewBag.Hash = Hash;
            ViewBag.Amount = Amount;
            ViewBag.StoreType = "3d_pay_hosting"; // Ödeme modelimiz biz buna göre anlatıyoruz 
            ViewBag.Description = "";
            ViewBag.XID = "";
            ViewBag.Lang = "tr";
            ViewBag.EMail = "destek@karayeltasarim.com";
            ViewBag.UserID = "karayelapi"; // bu id yi bankanın sanala pos ekranında biz oluşturuyoruz.
            ViewBag.PostURL = "http://localhost:44388/Firma/Tamamlandi";
            */

        }
        public ActionResult Tamamlandi() // Ödeme tammalnadığında çalışır.
        {
            return View();
        }
        public ActionResult Siparislerim() // Müşteri kendi siparişlerini listeler
        {
            var userId = (int)Session["LoginMusteriId"];
            var siparisler = db.SIPARIS.Where(x => x.siparis_user_id == userId).ToList();
            return View(siparisler);
        }
    }

}