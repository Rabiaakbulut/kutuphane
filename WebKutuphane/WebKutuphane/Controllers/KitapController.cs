using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebKutuphane.DAL;
using WebKutuphane.Models;
using WebKutuphane.Utils;

namespace WebKutuphane.Controllers
{
    public class KitapController : BaseController
    {
        private KutuphaneContext db = new KutuphaneContext();
        db_uyelikEntities db2 = new db_uyelikEntities();
        // GET: Kitap
        public ActionResult Index()
        {
            return View(db.Kitaplar.ToList());
        }

        [HttpGet]
        public PartialViewResult KitapVer()
        {
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult KitapVer(int id, string IP)
        {
            string s1 = IP;
            List<UyeBilgileri> üye = GetÜyeler();
            List<Kitap> kitap = GetKitaplar();
            foreach (var item in üye)
            {
                if (item.KullaniciAdi == s1 && item.KitapId!=null)
                {
                    foreach (var item2 in kitap)
                    {
                        if (item2.id == id&& item2.mevcutmu==false)
                        {
                            item.KitapId = null;
                            item2.mevcutmu = true;
                            db.SaveChanges();
                            db2.SaveChanges();
                        }
                    }
                }
            }
            return PartialView();
        }

        // GET: Kitap/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kitap kitap = db.Kitaplar.Find(id);
            if (kitap == null)
            {
                return HttpNotFound();
            }
            dynamic yorumlar = GetYorumlar(); //viewbag ile göndereceğimiz diğer model
            ViewBag.Yorumlar = yorumlar;
            dynamic üyeler = GetÜyeler(); //viewbag ile göndereceğimiz diğer model
            ViewBag.Üyeler = üyeler;
            return View(kitap);
        }

        //Veri tabanına SaveChanges ile kayıt yaparken dataread hatası vermemesi için direkt veri tabanından çağırmak yerine liste şeklinde çağırıyoruz
        public List<Yorum> GetYorumlar()  //Yorumları kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
            return db.Yorumlar.ToList();
        }
        public List<UyeBilgileri> GetÜyeler()  //Üyeleri kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
            return db2.UyeBilgileris.ToList();
        }
        public List<Kitap> GetKitaplar() 
        { 
            return db.Kitaplar.ToList();
        }

        // GET: Kitap/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kitap/Create
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ad,yazar,tür,dil,mevcutmu,resim,açıklama")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                db.Kitaplar.Add(kitap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kitap);
        }

        // GET: Kitap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kitap kitap = db.Kitaplar.Find(id);
            if (kitap == null)
            {
                return HttpNotFound();
            }
            return View(kitap);
        }

        // POST: Kitap/Edit/5
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ad,yazar,tür,dil,mevcutmu,resim,açıklama")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kitap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kitap);
        }


        // GET: Kitap/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kitap kitap = db.Kitaplar.Find(id);
            if (kitap == null)
            {
                return HttpNotFound();
            }
            return View(kitap);
        }

        // POST: Kitap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kitap kitap = db.Kitaplar.Find(id);
            db.Kitaplar.Remove(kitap);
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
