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
        public List<Yorum> GetYorumlar()  //Yorumları kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
            return db.Yorumlar.ToList();
        }
        public List<UyeBilgileri> GetÜyeler()  //Üyeleri kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
            return db2.UyeBilgileris.ToList();
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
