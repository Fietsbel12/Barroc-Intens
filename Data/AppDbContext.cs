using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BarrocIntens.Data
{
    internal class AppDbContext : DbContext
    {
        //TODO:optie voor model bespreek met groepje
        public DbSet<Medewerker> Medewerkers { get; set; }

        public DbSet<Koffiezetapparaat> Koffiezetapparaten { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(
        //        "server=localhost;user=root;password=;database=BarrocIntensDatabase",
        //        ServerVersion.Parse("8.0.30")
        //    );
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConfigurationManager.ConnectionStrings["BarrocIntensDatabase"].ConnectionString,ServerVersion.Parse("8.0.30"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------- MEDEWERKERS SEED ----------------------
            modelBuilder.Entity<Medewerker>().HasData(
    new Medewerker { Id = 1, Naam = "Pieter Eigenaar", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("eigenaar123"), MedewerkerRol = "Eigenaar" },
    new Medewerker { Id = 2, Naam = "Sophie Finance", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("finance123"), MedewerkerRol = "Finance" },
    new Medewerker { Id = 3, Naam = "Mark Sales", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("sales123"), MedewerkerRol = "Sales" },
    new Medewerker { Id = 4, Naam = "Laura Inkoop", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("inkoop123"), MedewerkerRol = "Inkoop" },
    new Medewerker { Id = 5, Naam = "Tom Maintenance", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("maintenance123"), MedewerkerRol = "Maintenance" },
    new Medewerker { Id = 6, Naam = "Emma Planner", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("planner123"), MedewerkerRol = "Planner" }
);


            // ---------------------- KOFFIEZETAPPARATEN SEED ----------------------
            var apparaten = new List<Koffiezetapparaat>();
            var random = new Random();

            // Voeg eerst alle apparaten toe met normale prijs
            for (int i = 1; i <= 10; i++)
            {
                apparaten.Add(new Koffiezetapparaat
                {
                    Id = i,
                    Naam = $"Koffiezetapparaat {i}",
                    Merk = i % 2 == 0 ? "Philips" : "DeLonghi",
                    Prijs = 99.99f + (i * 25),
                    Voorraad = 5 + i,
                    FotoPad = $@"FotoKoffiezetapparaatFolder\koffiezetapparaat{i}.jpeg"
                });
            }

            // Kies 4 willekeurige apparaten en zet hun prijs op 0
            var randomIndexes = new HashSet<int>();
            while (randomIndexes.Count < 4)
            {
                randomIndexes.Add(random.Next(0, apparaten.Count)); // 0 t/m 9
            }

            foreach (var index in randomIndexes)
            {
                apparaten[index].Prijs = 0f;
            }


            modelBuilder.Entity<Koffiezetapparaat>().HasData(apparaten);
        }
    }
}
