using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebKutuphane.Models;
using WebKutuphane.Utils;

namespace WebKutuphane.Controllers
{
    public class UyeController : BaseController
    {
        db_uyelikEntities1 db = new db_uyelikEntities1();//SQL
        // GET: Uye
        public ActionResult Index()
        {
            return View(db.UyeBilgileris.ToList());
        }

        [HttpGet]
        public PartialViewResult İadeCeza()
        {
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult İadeCeza(string cezaÖdeyen)
        {
            string paraÖdeyen = cezaÖdeyen;
            List<UyeBilgileri> üye = GetÜyeler();
            foreach (var item in üye)
            {
                if (item.KullaniciAdi == paraÖdeyen && item.Ceza==2)
                {
                    item.Ceza = 0;
                }
            }
            db.SaveChanges();
            Response.Redirect(Request.RawUrl);//iade yaptıktan sonra  sayfa yenileme
            return PartialView();
        }
        public List<UyeBilgileri> GetÜyeler()  //Üyeleri kitap details sayfasında görüntülemek için yorumları çağırıyoruz ve 
        {                               //viewbag sayesinde view sayfasına birden fazla model gönderiyoruz
            return db.UyeBilgileris.ToList();
        }

    }
}