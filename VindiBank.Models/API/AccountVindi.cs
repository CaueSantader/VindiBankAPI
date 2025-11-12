using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VindiBank.Models.Enum;

namespace VindiBank.Models.API
{
    public class AccountVindi
    {
        private int idConta { get; set; }
        public string nomeCliente { get; set; }

        private string _documento = string.Empty;
        public string documento
        {
            get => _documento;
            set => _documento = CleanDocument(value);
        }
        public PersonType PersonType { get; set; }
        private decimal _saldo { get; set; } = 1000;
        public DateTime dataAberturaConta { get; set; } = DateTime.UtcNow; 
        public string statusConta { get; set; }

        public string FormattedDocument => FormatDocument(documento, PersonType);
        public decimal saldo
        {
            get => _saldo;
            set => _saldo = (value == 0 ? 1000 : value);
        }

        private static string CleanDocument(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var onlyDigits = Regex.Replace(input, @"\D", "");
            return onlyDigits;
        }

        private static string FormatDocument(string document, PersonType type)
        {
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());

            try
            {
                return type switch
                {
                    PersonType.Individual when cleanDocument.Length == 11
                        => Convert.ToUInt64(cleanDocument).ToString(@"000\.000\.000\-00"),

                    PersonType.Business when cleanDocument.Length == 14
                        => Convert.ToUInt64(cleanDocument).ToString(@"00\.000\.000\/0000\-00"),

                    _ => cleanDocument 
                };
            }
            catch
            {

                return cleanDocument;
            }
        }
    }

}

