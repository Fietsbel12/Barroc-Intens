using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Offerte
    {
        public int Id { get; set; }
        public string Company {  get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PdfPath { get; set; }
    }
}
