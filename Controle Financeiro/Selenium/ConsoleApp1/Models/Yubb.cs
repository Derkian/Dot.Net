using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Yubb
    {
        public string categoria { get; set; } // Renda fixa, variável, fundos de investimento
        public string grupo { get; set; } // Tesouro Direto, CDB, LCI, LCA, etc
        public string titulo { get; set; } // Tesouro IPCA+ 2035
        public string risco { get; set; } // Alto médio baixo
        public DateTime data_resgate { get; set; } // 
        public decimal investimento { get; set; } // Valor investido
        public decimal retorno_1ano { get; set; }
        public decimal retorno_2anos { get; set; }
        public decimal retorno_3anos { get; set; }
        public decimal retorno_4anos { get; set; }
        public decimal retorno_5anos { get; set; }
        public decimal retorno_10anos { get; set; }
        public decimal retorno_15anos { get; set; }
        public decimal retorno_20anos { get; set; }
        public decimal retorno_25anos { get; set; }
        public decimal retorno_30anos { get; set; }
    }
}
