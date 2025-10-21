using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BarrocIntensTEST.Data
{
    internal class AppDbContext : DbContext
    {
        
        //public DbSet<Medewerker> Medewerkers { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(
        //        "server=localhost;user=root;password=;database=BarrocIntensTESTDatabase",
        //        ServerVersion.Parse("8.0.30")
        //    );
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConfigurationManager.ConnectionStrings["BarrocIntensDatabase"].ConnectionString,ServerVersion.Parse("8.0.30"));
        }
        

    }
}
