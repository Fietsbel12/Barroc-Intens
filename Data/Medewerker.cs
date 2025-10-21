using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Medewerker
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Wachtwoord { get; set; }
        public string MedewerkerRol { get; set; }
    }
}
