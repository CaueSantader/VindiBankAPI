using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VindiBank.Business.Configurations
{
    public class AppSettings
    {
        private readonly IConfiguration _configuration;
        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConfiguracao(string campo) => _configuration.GetSection(campo).Value;
        public string GetConnection(string campo) => _configuration.GetConnectionString(campo);
    }
}
