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
        db_uyelikEntities db = new db_uyelikEntities();//SQL
        // GET: Uye
        public ActionResult Index()
        {
            return View(db.UyeBilgileris.ToList());
        }
    }
}