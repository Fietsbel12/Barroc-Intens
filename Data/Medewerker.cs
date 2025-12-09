using System.Collections.Generic;

namespace BarrocIntens.Data
{
    public class Medewerker
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Wachtwoord { get; set; }
        public string MedewerkerRol { get; set; }

        public ICollection<Taken> Taken { get; set; }
    }
}
