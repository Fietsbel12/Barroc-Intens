using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BarrocIntens.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Medewerker> Medewerkers { get; set; }
        public DbSet<Taken> Taken { get; set; }
        public DbSet<Klant> Klanten { get; set; }
        public DbSet<Koffiezetapparaat> Koffiezetapparaten { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                ConfigurationManager.ConnectionStrings["BarrocIntensDatabase"].ConnectionString,
                ServerVersion.Parse("8.0.30")
            );
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

            // ---------------------- KLANT SEED ----------------------
            modelBuilder.Entity<Klant>().HasData(
                new Klant { Id = 1, KlantNaam = "Klant A", Adres = "Straat 1, Stad", TelefoonNummer = "0612345678", Email = "Test@gmail.com" }
            );

            // ---------------------- KOFFIEZETAPPARATEN SEED ----------------------
            var apparaten = new List<Koffiezetapparaat>();
            var random = new Random();

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

            var randomIndexes = new HashSet<int>();
            while (randomIndexes.Count < 4)
            {
                randomIndexes.Add(random.Next(0, apparaten.Count));
            }

            foreach (var index in randomIndexes)
            {
                apparaten[index].Prijs = 0f;
            }

            modelBuilder.Entity<Koffiezetapparaat>().HasData(apparaten);

            // ---------------------- TAKEN SEED ----------------------
            int medewerkerCounter = 1;
            var taken = new List<Taken>();

            for (int id = 1; id <= 20; id++)
            {
                taken.Add(new Taken
                {
                    Id = id,
                    Name = $"Taak {id}",
                    Description = $"Beschrijving taak {id}",
                    Tijd = DateTime.Now.AddHours(id),
                    MedewerkerId = medewerkerCounter
                });

                medewerkerCounter++;
                if (medewerkerCounter > 6)
                    medewerkerCounter = 1;
            }

            modelBuilder.Entity<Taken>().HasData(taken);
        }
    }
}
