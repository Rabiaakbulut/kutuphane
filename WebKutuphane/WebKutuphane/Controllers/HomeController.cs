using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebKutuphane.DAL;
using WebKutuphane.Models;

namespace WebKutuphane.Controllers
{
    public class HomeController : Controller
    {
        db_uyelikEntities1 db = new db_uyelikEntities1(); //üyelik, login, signup, logout
        KutuphaneContext db2 = new KutuphaneContext();// entity framework -> kitaplar ve yorumlar

        // GET: Home
        public ActionResult Index()
        {
            return View(db2.Kitaplar.ToList());
        }

        //seçilen özelliklere göre kitapları filtreleme
        [HttpPost]
        public ActionResult Index(List<string> tür, List<string> dil,string mevcutmu)
        {          
            List<Kitap> kitaplar = new List<Kitap>();

            if (tür == null)
            {
                tür = new List<string>();//null listeyi başlatmadan önce eleman eklenemiyor
                tür.Add("Edebiyat");
                tür.Add("ÇocukVeGençlik");
                tür.Add("AraştırmaTarih");
                tür.Add("SanatTasarım");
                tür.Add("Felsefe");
                tür.Add("Bilim");
                tür.Add("ÇizgiRoman");
            }

            if (dil == null)
            {
                dil = new List<string>();
                dil.Add("Türkçe");
                dil.Add("İngilizce");
                dil.Add("Almanca");
                dil.Add("Diğer");
            }

            if (mevcutmu=="evet")
            {
                foreach (var kitap in GetKitaplar())
                {
                    if (tür.Contains(kitap.tür) && dil.Contains(kitap.dil) && kitap.mevcutmu == true)
                        kitaplar.Add(kitap);
                }
            }
            else if(mevcutmu=="hayır")
            {
                foreach (var kitap in GetKitaplar())
                {
                    if (tür.Contains(kitap.tür) && dil.Contains(kitap.dil) && kitap.mevcutmu == false)
                        kitaplar.Add(kitap);
                }
            }
            else
            {
                foreach (var kitap in GetKitaplar())
                {
                    if (tür.Contains(kitap.tür) && dil.Contains(kitap.dil))
                        kitaplar.Add(kitap);
                }
            }
            return View(kitaplar);
        }
        public ActionResult Welcome()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult YorumYap()
        {
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult YorumYap(Yorum y,int id)
        {
            y.KitapId = id;
            y.KullaniciAdi = Session["KullaniciAdiSS"].ToString();
            db2.Yorumlar.Add(y);
            db2.SaveChanges();
            Response.Redirect(Request.RawUrl);//yorum yaptıktan sonra yorumun gözükmesi için sayfa yenileme
            return PartialView();
        }

        public List<UyeBilgileri> GetÜyeler() 
        {                               
            return db.UyeBilgileris.ToList();
        }
        public List<Kitap> GetKitaplar()
        {
            return db2.Kitaplar.ToList();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(UyeBilgileri uyeBilgileri) //Üye kaydı
        {
            uyeBilgileri.KitapId = null;
            uyeBilgileri.KitapAlımTarihi = null;
            uyeBilgileri.Ceza = 0;

            if (db.UyeBilgileris.Any(x => x.KullaniciAdi == uyeBilgileri.KullaniciAdi))//girilen kullanıcı adına ait bir kayıt varsa uyarı ver
            {
                ViewBag.uyarı1 = "Kullanıcı adı alınmış";
                return View();
            }
            else if (uyeBilgileri.KullaniciSifre!=uyeBilgileri.KullaniciSifreTekrar)
            {
                ViewBag.uyarı2 = "Şifreler uyuşmuyor";
                return View();
            }   
            else//kullanıcı adı alınmamışsa ve şifreler uyuşuyorsa üyeliği veri tabanına kaydet
            {
                db.UyeBilgileris.Add(uyeBilgileri);
                db.SaveChanges();

                Session["KullaniciIdSS"] = uyeBilgileri.Id.ToString();
                Session["KullaniciAdiSS"] = uyeBilgileri.KullaniciAdi.ToString();

                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();//KullanıcıId, KullanıcıAdı ve Admin bilgilerini temizliyoruz
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//dış saldırılara karşı önlem
        public ActionResult Login(UyeBilgileri uyeBilgileri)
        {
            //girilen kullanıcı adı ve şifreyi al veri tabnındaki bilgilerle eşleştir
            var checkLogin = db.UyeBilgileris.Where(x => x.KullaniciAdi.Equals(uyeBilgileri.KullaniciAdi) && x.KullaniciSifre.Equals(uyeBilgileri.KullaniciSifre)).FirstOrDefault();

            if (checkLogin != null) //Girilen kullanıcı adı ve şifreye ait veri tabanında kayıt varsa
            {
                Session["KullaniciIdSS"] = uyeBilgileri.Id.ToString();
                Session["KullaniciAdiSS"] = uyeBilgileri.KullaniciAdi.ToString();

                if (uyeBilgileri.KullaniciAdi.ToString() == "admin")//adminse
                {
                    Session["Admin"] = "1"; //BaseController
                    return RedirectToAction("Index", "Uye");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else //kayıt yoksa
            {
                ViewBag.uyarı = "yanlış kullanıcı adı veya şifre";
            }
            return View();
        }


    }
}