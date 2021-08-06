using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebKutuphane.Models;

namespace WebKutuphane.DAL
{
    public class KutuphaneInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<KutuphaneContext>
    {
        protected override void Seed(KutuphaneContext context)
        {
            var kitaplar = new List<Kitap>
            {
            new Kitap{ad="Kürk Mantolu Madonna",yazar="Sabahattin Ali",tür="Roman",dil="Türkçe",mevcutmu=true,resim="2020.jpg",açıklama="güzel 1bir kitap kmjmfdj fdj kdf jdmfjk f"},
            new Kitap{ad="Faust",yazar="Johann Wolfgang von Goethe",tür="Tiyatro",dil="Türkçe çeviri",mevcutmu=true,resim="2020.jpg",açıklama="güzel 2bir kitap kmjmfdj fdj kdf jdmfjk f"},
            new Kitap{ad="denemee",yazar="Johann Wolfgang von Goethe",tür="Tiyatro",dil="Türkçe çeviri",mevcutmu=true,resim="2020.jpg",açıklama="güzel 3bir kitap kmjmfdj fdj kdf jdmfjk f"},
            new Kitap{ad="denemdddee",yazar="Johann Wolfgang von Goethe",tür="Tiyatro",dil="Türkçe çeviri",mevcutmu=true,resim="2020.jpg",açıklama="güzel 4bir kitap kmjmfdj fdj kdf jdmfjk f"}
            };
            kitaplar.ForEach(s => context.Kitaplar.Add(s));
            context.SaveChanges();

            var yorumlar = new List<Yorum>
            {
            new Yorum {KitapId=1,KullaniciAdi="admin",Metin="kfkdsnfn jdddsj n gngdgnn"},
            new Yorum {KitapId=2,KullaniciAdi="admin",Metin="k2222dsj n gngdgnn"},
            new Yorum {KitapId=2,KullaniciAdi="admin",Metin="kf3333dsj n gngdgnn"},
            };

            yorumlar.ForEach(s => context.Yorumlar.Add(s));
            context.SaveChanges();
        }
    }
}