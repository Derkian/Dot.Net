using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Investimento
    {
        public string risco { get; set; }
        public string tipo { get; set; }
        public string nome { get; set; }
        public DateTime mes { get; set; }
        public double valor { get; set; }
    }
}
