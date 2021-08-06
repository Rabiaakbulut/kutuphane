using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebKutuphane.Models
{
    public class Yorum
    {
        public int Id { get; set; }
        public int KitapId { get; set; }
        public string KullaniciAdi { get; set; }//bunun yerine kullanıcı adi veya ekstra olarak?
        public string Metin { get; set; }
        
    }
}