using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public DbSet<Taken> Taken { get; set; }


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
            // ---------------------- TAKEN SEED ----------------------
            modelBuilder.Entity<Taken>().HasData(
                new Taken { Id = 1, Name = "Taak 1", Description = "Beschrijving taak 1", Tijd = DateTime.Now.AddHours(1) },
                new Taken { Id = 2, Name = "Taak 2", Description = "Beschrijving taak 2yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", Tijd = DateTime.Now.AddHours(2) },
                new Taken { Id = 3, Name = "Taak 3", Description = "Beschrijving taak 3", Tijd = DateTime.Now.AddHours(3) },
                new Taken { Id = 4, Name = "Taak 4", Description = "Beschrijving taak 4", Tijd = DateTime.Now.AddHours(4) },
                new Taken { Id = 5, Name = "Taak 5", Description = "Beschrijving taak 5", Tijd = DateTime.Now.AddHours(5) },
                new Taken { Id = 6, Name = "Taak 6", Description = "Beschrijving taak 6", Tijd = DateTime.Now.AddHours(6) },
                new Taken { Id = 7, Name = "Taak 7", Description = "Beschrijving taak 7", Tijd = DateTime.Now.AddHours(7) },
                new Taken { Id = 8, Name = "Taak 8", Description = "Beschrijving taak 8", Tijd = DateTime.Now.AddHours(8) },
                new Taken { Id = 9, Name = "Taak 9", Description = "Beschrijving taak 9", Tijd = DateTime.Now.AddHours(9) },
                new Taken { Id = 10, Name = "Taak 10", Description = "Beschrijving taak 10", Tijd = DateTime.Now.AddHours(10) },

                // Extra 10 taken
                new Taken { Id = 11, Name = "Taak 11", Description = "Beschrijving taak 11", Tijd = DateTime.Now.AddHours(11) },
                new Taken { Id = 12, Name = "Taak 12", Description = "Beschrijving taak 12", Tijd = DateTime.Now.AddHours(12) },
                new Taken { Id = 13, Name = "Taak 13", Description = "Beschrijving taak 13", Tijd = DateTime.Now.AddHours(13) },
                new Taken { Id = 14, Name = "Taak 14", Description = "Beschrijving taak 14", Tijd = DateTime.Now.AddHours(14) },
                new Taken { Id = 15, Name = "Taak 15", Description = "Beschrijving taak 15", Tijd = DateTime.Now.AddHours(15) },
                new Taken { Id = 16, Name = "Taak 16", Description = "Beschrijving taak 16", Tijd = DateTime.Now.AddHours(16) },
                new Taken { Id = 17, Name = "Taak 17", Description = "Beschrijving taak 17", Tijd = DateTime.Now.AddHours(17) },
                new Taken { Id = 18, Name = "Taak 18", Description = "Beschrijving taak 18", Tijd = DateTime.Now.AddHours(18) },
                new Taken { Id = 19, Name = "Taak 19", Description = "Beschrijving taak 19", Tijd = DateTime.Now.AddHours(19) },
                new Taken { Id = 20, Name = "Taak 20", Description = "Beschrijving taak 20", Tijd = DateTime.Now.AddHours(20) }
            );


        }
    }
}
