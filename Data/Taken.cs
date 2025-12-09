using System;

namespace BarrocIntens.Data
{
    public class Taken
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Tijd { get; set; }

        public int MedewerkerId { get; set; }
        public Medewerker Medewerker { get; set; }

        // Read-only property voor x:Bind
        public string TijdString => Tijd.ToString("dd-MM-yyyy HH:mm");
    }
}
