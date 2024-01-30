using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleCarteira.Domain
{
    public class Carteira
    {
        public string Ticker { get; set; }
        public int Quantidade { get; set; }
        public double PrecoMedio { get; set; }
        public double ValorTotal { get; set; }


    }
}
