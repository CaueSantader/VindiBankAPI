

using VindiBank.Models.API;

namespace VindiBank.Repository.Repositories
{
    public interface IRequisicoesRepository
    {
        public void IncludeNewClients(string connString, object oEntrada);
        AccountVindi? SearchClients(string connString, object oEntrada);
        public void DisablesClients(string connString, object oEntrada);
        public string? TransitionClientes(string connString, object oEntrada);

    }
}
