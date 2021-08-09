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
        private KutuphaneContext db2 = new KutuphaneContext();// entity framework -> kitaplar ve yorumlar
        // GET: Home
        public ActionResult Index()
        {
            return View(db2.Kitaplar.ToList());

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



        [HttpGet]
        public PartialViewResult KitapAl()
        {
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult KitapAl(int id)
        {
            List<UyeBilgileri> üye = GetÜyeler();
            List<Kitap> kitap = GetKitaplar();
            foreach (var item in üye)
            {
                if (item.KullaniciAdi == Session["KullaniciAdiSS"].ToString() && item.KitapId == null && item.Ceza<2)
                {
                    item.KitapId = id;
                    item.KitapAlımTarihi = DateTime.Now;
                    foreach (var item2 in kitap)
                    {
                        if (item2.id == id && item2.mevcutmu == true)
                        {
                            item2.mevcutmu = false;
                            db.SaveChanges();
                            db2.SaveChanges();
                        }
                        //else
                        //{
                        //   //üzerinde birden fazla kitap varsa hata mesajı ver ya da kitap alma butonunu aktif etme
                        //}
                    }
                }
            }
            return PartialView();
        }

        public List<UyeBilgileri> GetÜyeler()  //Üyeleri kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
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
        public ActionResult Signup(UyeBilgileri uyeBilgileri)
        {
            uyeBilgileri.KitapId = null;//üye olurken üzerine kayıtlı kitap olmasın
            uyeBilgileri.KitapAlımTarihi = null;
            uyeBilgileri.Ceza = 0;
            if (db.UyeBilgileris.Any(x => x.KullaniciAdi == uyeBilgileri.KullaniciAdi))//girilen kullanıcı adına ait bir kayıt varsa uyarı ver
            {
                ViewBag.uyarı1 = "Kullanıcı adı alınmış";
                return View();
            }
            else if (uyeBilgileri.KullaniciSifre!=uyeBilgileri.KullaniciSifreTekrar)//şifreler farklıysa uyarı ver
            {
                ViewBag.uyarı2 = "Şifreler uyuşmuyor";
                return View();
            }   
            else//kullanıcı adı alınmamışsa ve şifreler uyuşıyorsa üyeliği veri tabanına kaydet
            {
                db.UyeBilgileris.Add(uyeBilgileri);
                db.SaveChanges();
                //Sessionlar her kullanıcı için özel verileri tutmayı sağlar
                Session["KullaniciIdSS"] = uyeBilgileri.Id.ToString();
                Session["KullaniciAdiSS"] = uyeBilgileri.KullaniciAdi.ToString();//home index sayfasında "Merhaba kullanıcıAdı"
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();//KullanıcıId, KullanıcıAdı ve Amin bilgilerini temizliyoruz
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
                    Session["Admin"] = "1"; //BaseController'dan miras alan sayfalara erişim için 1 yap
                    return RedirectToAction("Index", "Uye");//admin görüntülesin
                }
                else
                {
                    return RedirectToAction("Index", "Home");//herkes
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