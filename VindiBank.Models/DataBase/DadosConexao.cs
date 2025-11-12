using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VindiBank.Models.DataBase
{
    public class DadosConexao
    {
        public string ip { get; set; }
        public string banco { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string porta 
        {
            get { return "5432"; }

            set { this.porta = value; }
         }


    }
}
