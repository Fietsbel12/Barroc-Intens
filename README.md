# Barroc-Intens
Dit is de uitwerking van de eindopdracht voor E3 Barroc Intens. De applicatie is een C# desktop-oplossing (WPF) die fungeert als CRM- en ERP-systeem voor een koffiemachineleverancier. Het pakket bevat Entity Framework Core integratie en geautomatiseerde tests.

## Over de Applicatie
Barroc Intens is een intern bedrijfsbeheersysteem voor een koffiemachine-verhuurbedrijf. De applicatie is exclusief bedoeld voor medewerkers en ondersteunt het volledige proces van het verhuren van apparaten aan klanten en de bijbehorende planning.

## Status van de vereisten
- [x] Werkende Applicatie
- [x] Database Migraties
- [x] Data Seeders
- [x] Acceptatietests

## Aan de slag

### 1. Vereisten
* .NET SDK 8.0
* Visual Studio 2022
* SQL Server LocalDatabase

### 2. Installatie & Database Setup
Deze applicatie maakt gebruik van en bevat **Seeders** voor testdata. 
**Belangrijk:** Zorg ervoor dat de App.config wordt aangemaakt met de juiste connectionstring naar jouw SQL Server LocalDB instantie. Dupliceer de .Example file.

1. Clone de repository:
git clone https://github.com/Fietsbel12/Barroc-Intens


2. Open de **Package Manager Console** in Visual Studio en voer het volgende commando uit:
Update-Database


### 3. De applicatie starten
Start het project via de 'Start' knop in Visual Studio of gebruik het volgende commando in de terminal:
dotnet run

### Overzicht van de applicatie

### Gebruikersrollen
De applicatie ondersteunt verschillende rollen met specifieke rechten:
* **Eigenaar / Sales:** Beheer van klanten en offertes.
* **Maintenance / Planner:** Inplannen en uitvoeren van onderhoud aan koffiezetapparaten.
* **Finance:** Facturatie en betalingsbeheer.
* **Inkoop:** Voorraadbeheer.

### Test-Inloggegevens
Gebruik de volgende gegevens om de verschillende onderdelen van de applicatie te testen:
* **Pieter Eigenaar** - Wachtwoord: `eigenaar123` - Rol: Eigenaar
* **Sophie Finance** - Wachtwoord: `finance123` - Rol: Finance
* **Mark Sales** - Wachtwoord: `sales123` - Rol: Sales
* **Laura Inkoop** - Wachtwoord: `inkoop123` - Rol: Inkoop
* **Tom Maintenance** - Wachtwoord: `maintenance123` - Rol: Maintenance
* **Emma Planner** - Wachtwoord: `planner123` - Rol: Planner

### Projectstructuur
De applicatie is modulair opgebouwd voor een duidelijke scheiding tussen data en gebruikersinterface:
* **Data/**: Bevat de `AppDbContext.cs` en alle entiteitsmodellen zoals `Klant.cs`, `Koffiezetapparaat.cs` en `Taken.cs`.
* **View/**: Bevat de `.xaml` bestanden voor de verschillende rollen, zoals `FinanceHomePage`, `MaintenanceHomepage` en `InkoopHomepage`.
* **FotoKoffiezetapparaatFolder/**: Opslaglocatie voor de productafbeeldingen die binnen de applicatie worden gebruikt.
* **Assets/**: Bevat statische bronnen zoals iconen en stijlen.

### Bekende beperkingen of aandachtspunten
* **OS:** De applicatie is gebouwd met WPF en werkt daardoor alleen op Windows en Pages.
* **Database:** De connectie is ingesteld op `(localdb)\mssqllocaldb`. Bij gebruik van een andere SQL-server moet de connectionstring in de `Data` map worden aangepast.
* **Afbeeldingen:** Nieuwe productfoto's moeten handmatig in de map `FotoKoffiezetapparaatFolder` worden geplaatst om zichtbaar te zijn in de applicatie.