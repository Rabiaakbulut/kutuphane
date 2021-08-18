using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
        db_uyelikEntities1 db2 = new db_uyelikEntities1();
        // GET: Kitap
        public ActionResult Index()
        {
            return View(db.Kitaplar.ToList());
        }
        [HttpPost]
        public ActionResult Index(string search) //Kitap index sayfasında kitap arama
        {
            search = search.ToLower();
            List<Kitap> kitap = GetKitaplar();
            List<Kitap> kitapFiltre = new List<Kitap>();
            foreach (var item in kitap)
            {
                if(item.tür.ToLower().Contains(search) || item.yazar.ToLower().Contains(search) || item.ad.ToLower().Contains(search) || item.dil.ToLower().Contains(search))
                {
                    kitapFiltre.Add(item);
                }
            }
            return View(kitapFiltre.ToList());
        }
        [HttpGet]
        public PartialViewResult KitapVer()
        {
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult KitapVer(int id, string iadeEden)
        {
            string kitabiVeren= iadeEden;
            List<UyeBilgileri> üye = GetÜyeler();
            List<Kitap> kitap = GetKitaplar();
            foreach (var item in üye)
            {
                if (item.KullaniciAdi == kitabiVeren && item.KitapId!=null)
                {
                    foreach (var item2 in kitap)
                    {
                        if (item2.id == id&& item2.mevcutmu==false)
                        {
                            item.KitapId = null;
                            
                            item2.mevcutmu = true;
                            //item.kitapAlımTarihi nullable şekilde tanımlandığı için TimeSpan? oluyor ve timespan? days özelliği olmadığı için bu şkelil tanımlıyoruz
                            if ((DateTime.Now - (item.KitapAlımTarihi != null ? item.KitapAlımTarihi : DateTime.Now)).Value.Days > 20)
                            {
                                if (item.Ceza == null)
                                    item.Ceza = 1;
                                else
                                    item.Ceza += 1;
                            }
                            item.KitapAlımTarihi = null;//ceza ekleeeeee3
                            db.SaveChanges();
                            db2.SaveChanges();
                        }
                    }
                }
            }
            return PartialView();
        }

        public ActionResult KitapAl()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KitapAl(int id)
        {
            Kitap kitap = db.Kitaplar.Find(id);
            List<UyeBilgileri> üye = GetÜyeler();
            foreach (var item in üye)
            {
                if (item.KullaniciAdi == Session["KullaniciAdiSS"].ToString() && item.KitapId == null && item.Ceza < 2)
                {
                    item.KitapId = id;
                    item.KitapAlımTarihi = DateTime.Now;
                    if (kitap.mevcutmu == true)
                    {
                        kitap.mevcutmu = false;
                        db.SaveChanges();
                        db2.SaveChanges();
                    }
                    //else
                    //{
                    //   //üzerinde birden fazla kitap varsa hata mesajı ver ya da kitap alma butonunu aktif etme
                    //}
                }
            }
            return View();
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
        public ActionResult Create([Bind(Include = "id,ad,yazar,tür,mevcutmu,resim")] Kitap kitap,string dil,string tür)
        {
            if (ModelState.IsValid)
            {
                kitap.dil = dil;
                kitap.tür = tür;
                string kitabınAdı = kitap.ad;
                try
                {
                    WebClient client = new WebClient();
                    string strPgeCode = client.DownloadString($"https://tr.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles={kitabınAdı}");
                    dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPgeCode);
                    string temp = (string)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dobj["query"]["pages"]).First).First).Last;
                    kitap.açıklama = temp;
                }
                catch
                {
                    kitap.açıklama = "Bilgi bulunamadı";
                }

                //string fileName = Path.GetFileNameWithoutExtension(kitap.resim);
                //string extension = Path.GetExtension(kitap.resim);
                //fileName = fileName + extension;
                //kitap.resim = "/img/"+fileName;
                //fileName = Path.Combine(Server.MapPath("/img/"), fileName);



                //if (Request.Files.Count > 0)
                //{
                //    var fileSavePath = "";
                //    var uploadedFile = Request.Files[0];
                //    string fileName = Path.GetFileName(uploadedFile.FileName);
                //    fileSavePath = Path.Combine("Image", fileName);
                //    Request.Files[0].SaveAs(Server.MapPath(fileSavePath));
                //    kitap.resim = fileSavePath;
                //}
                //}
                //if (Request.Files.Count > 0)
                //    {
                //        string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                //        string uzanti = Path.GetExtension(Request.Files[0].FileName);
                //        string yol = "~/Image/" + dosyaadi + uzanti;
                //        Request.Files[0].SaveAs(Server.MapPath(yol));
                //        kitap.resim = "/Image/" + dosyaadi + uzanti;
                //    }
                kitap.mevcutmu = true;
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
        public ActionResult Edit([Bind(Include = "id,ad,yazar,tür,dil,mevcutmu,açıklama,resim")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                //WebClient client = new WebClient();
                //string deneme = kitap.ad;
                //string strPgeCode = client.DownloadString($"https://tr.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles={deneme}");
                //dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPgeCode);
                //string temp = (string)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dobj["query"]["pages"]).First).First).Last;
                //kitap.açıklama = temp;
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
