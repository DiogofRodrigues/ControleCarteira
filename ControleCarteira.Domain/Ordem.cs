using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleCarteira.Domain
{
    public class Ordem
    {
        public string Ticker { get; set; }
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
        public DateTime DataOrdem { get; set; }


        public override string ToString()
        {
            var message = $"{{ \"Ticker\": \'{this.Ticker}\', \"Tipo\": '{this.Tipo}', \"Quantidade\": '{this.Quantidade}', \"PrecoUnitario\": '{this.PrecoUnitario}', \"DataOrdem\": '{this.DataOrdem}'  }}";
            return message;
        }
    }
}
