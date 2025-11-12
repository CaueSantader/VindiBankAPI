using Microsoft.Extensions.Configuration;
using VindiBank.Business.Configurations;
using VindiBank.Business.Interfaces;
using VindiBank.Models.API;
using VindiBank.Models.Enum;
using VindiBank.Repository.Repositories;

namespace VindiBank.Business.Controll
{
    public class FluxoAPIControll : IFluxoAPIControll
    {
        private readonly IRequisicoesRepository _requisicoesRepository;
        private readonly Dictionary<MetodoAPI, Func<object, string, Task<object?>>> _handlers;
        public FluxoAPIControll(IRequisicoesRepository requisicoesRepository) 
        {
            _requisicoesRepository = requisicoesRepository;
            _handlers = new Dictionary<MetodoAPI, Func<object, string, Task<object?>>>
            {

                { MetodoAPI.createAccount, Wrap((input, conn) => _requisicoesRepository.IncludeNewClients(conn, input)) },

                { MetodoAPI.getAccounts, WrapFunc((input, conn) => _requisicoesRepository.SearchClients(conn, input)) },

                { MetodoAPI.accountDeactivation, Wrap((input, conn) => _requisicoesRepository.DisablesClients(conn, input)) },

                { MetodoAPI.accountTransfer, WrapString((input, conn) => _requisicoesRepository.TransitionClientes(conn, input)) }
            };
        }
        
        private static Func<object, string, Task<object?>> Wrap(Action<object, string> action) =>
        (input, conn) =>
        {
            action(input, conn);
            return Task.FromResult<object?>(null);
        };

        private static Func<object, string, Task<object?>> WrapFunc(Func<object, string, object?> func) =>
        (input, conn) =>
        {
            var result = func(input, conn);
            return Task.FromResult(result);
        };

        private static Func<object, string, Task<object?>> WrapString(Func<object, string, object?> func) =>
       (input, conn) =>
       {
           var result = func(input, conn);
           return Task.FromResult(result);
        };

        public async Task<object?> ProcessaDados(MetodoAPI metodoAPI, object oEntrada, IConfiguration configuration)
        {
            AppSettings appSettings = new AppSettings(configuration);

            var connString = appSettings.GetConnection("DefaultConnection");

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));


            if (!_handlers.TryGetValue(metodoAPI, out var handler))
                throw new ArgumentException($"Método não suportado: {metodoAPI}", nameof(metodoAPI));

            var resultado = await handler(oEntrada, connString);

            return resultado;

        }

    }
}
