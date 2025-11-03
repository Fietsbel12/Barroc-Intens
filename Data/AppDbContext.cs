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

            modelBuilder.Entity<Medewerker>().HasData(
                new Medewerker
                {
                    Id = 1,
                    Naam = "Pieter Eigenaar",
                    Wachtwoord = "eigenaar123",
                    MedewerkerRol = "Eigenaar"
                },
                new Medewerker
                {
                    Id = 2,
                    Naam = "Sophie Finance",
                    Wachtwoord = "finance123",
                    MedewerkerRol = "Finance"
                },
                new Medewerker
                {
                    Id = 3,
                    Naam = "Mark Sales",
                    Wachtwoord = "sales123",
                    MedewerkerRol = "Sales"
                },
                new Medewerker
                {
                    Id = 4,
                    Naam = "Laura Inkoop",
                    Wachtwoord = "inkoop123",
                    MedewerkerRol = "Inkoop"
                },
                new Medewerker
                {
                    Id = 5,
                    Naam = "Tom Maintenance",
                    Wachtwoord = "maintenance123",
                    MedewerkerRol = "Maintenance"
                },
                new Medewerker
                {
                    Id = 6,
                    Naam = "Emma Planner",
                    Wachtwoord = "planner123",
                    MedewerkerRol = "Planner"
                }
            );

        }
}}
