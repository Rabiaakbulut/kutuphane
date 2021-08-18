using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebKutuphane.Utils
{
    public class BaseController : System.Web.Mvc.Controller //Controller sınıfından miras aldık
    {
        //Controller olarak BaseControllerdan miras alanların fonksiyonları çalışmadan önce burası çalışacak
        //Admin modundaysak (Session["Admin"]=1) istenilen sayfa açılacak değilsek açılmayacak
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            bool isControl = controllerName == "Kitap" && actionName == "Details" || controllerName=="Home" || controllerName == "Uye" && actionName == "UyeProfil" || controllerName == "Yorum" && actionName == "Delete";

            if (!isControl)//Home controller ve Kitap controllerinin Details sayfası dışındakilere sadece admin erişebilsin
            {
                if (Session["Admin"] == null || Session["Admin"].ToString() != "1")
                {
                    filterContext.Result = new RedirectResult("/Home/Login");//admin değilse admin olarak giriş yapması için login sayfasına yönlensin
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}