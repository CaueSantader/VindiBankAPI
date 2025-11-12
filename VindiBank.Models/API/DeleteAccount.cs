using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VindiBank.Models.Enum;

namespace VindiBank.Models.API
{
    public class DeleteAccount
    {
        public string? documento { get; set; } = string.Empty;
        private PersonType PersonType { get; set; }

        public string FormattedDocument => FormatDocument(documento, PersonType);

        private static string FormatDocument(string document, PersonType type)
        {
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());

            return type switch
            {
                PersonType.Individual => Convert.ToUInt64(cleanDocument).ToString(@"000\.000\.000\-00"),
                PersonType.Business => Convert.ToUInt64(cleanDocument).ToString(@"00\.000\.000\/0000\-00"),
                _ => document
            };
        }
    }
}
