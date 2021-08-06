using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebKutuphane.Models
{
    public class Kitap
    {
        public int id { get; set; }
        public string ad { get; set; }
        public string yazar { get; set; }
        public string tür { get; set; }
        public string dil { get; set; }
        public bool mevcutmu { get; set; }
        public string resim { get; set; }
        public string açıklama { get; set; }
    }
}