using BarrocIntens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Cartitem
    {
        public Koffiezetapparaat Product { get; set; }
        public int Aantal { get; set; }
        public decimal TotaalPrijs => (decimal)Product.Prijs * Aantal;
    }
}
