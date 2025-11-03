using System.ComponentModel.DataAnnotations;

namespace BarrocIntens.Models
{
    public class Koffiezetapparaat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Naam { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Merk { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 9999.99, ErrorMessage = "Prijs moet tussen 0 en 9999 liggen.")]
        public float Prijs { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Voorraad mag niet negatief zijn.")]
        public int Voorraad { get; set; }

        [MaxLength(500)]
        public string? FotoPad { get; set; }  // Pad naar de geselecteerde foto
    }
}
