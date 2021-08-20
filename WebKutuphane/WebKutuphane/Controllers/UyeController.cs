﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebKutuphane.DAL;
using WebKutuphane.Models;
using WebKutuphane.Utils;

namespace WebKutuphane.Controllers
{
    public class UyeController : BaseController
    {
        db_uyelikEntities1 db = new db_uyelikEntities1();//SQL
        private KutuphaneContext db2 = new KutuphaneContext();
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
            Response.Redirect(Request.RawUrl);
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

        public ActionResult UyeProfil()
        {
            if (Session["KullaniciAdiSS"] != null)
            {
                List<Kitap> kitap = GetKitaplar();
                List<UyeBilgileri> üye = GetÜyeler();
                UyeBilgileri AktifUye = new UyeBilgileri();
                foreach (var item in üye)
                {
                    if (item.KullaniciAdi == Session["KullaniciAdiSS"].ToString())
                    {
                        AktifUye = item;
                    }
                }

                if (AktifUye.KitapId != null)
                {
                    foreach (var item in kitap)
                    {
                        if (item.id == AktifUye.KitapId)
                        {
                            ViewBag.kitapAdı = item.ad;
                            ViewBag.kitapId = item.id;
                        }
                    }
                }
                return View(AktifUye);
            }
            else
            {
                return View();
            }
            
        }

    }
}