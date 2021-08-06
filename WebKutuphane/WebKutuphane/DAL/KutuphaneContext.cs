using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebKutuphane.Models;

namespace WebKutuphane.DAL
{
    public class KutuphaneContext : DbContext
    {
        public KutuphaneContext() : base("KutuphaneContext") // Web. config 
        {
        }

        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}