using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VindiBank.Models.Enum;

namespace VindiBank.Models.API
{
    public class TransferBetweenAccounts
    {
        public string documentoEntrada { get; set; } = string.Empty;
        public string documentoSaida { get; set; } = string.Empty;
        public bool statusConta { get; set; } = true;
        public decimal saldoDocumentoEntrada { get; set; }
        public decimal saldoDocumentoSaida { get; set; }
        private decimal Amount { get; set; }

    }
}
