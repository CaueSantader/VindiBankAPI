using Microsoft.Extensions.Configuration;
using VindiBank.Models.API;
using VindiBank.Models.Enum;
namespace VindiBank.Business.Interfaces
{
    public interface IFluxoAPIControll
    {
        Task <Object?> ProcessaDados(MetodoAPI metodoAPI, object oEntrada, IConfiguration configuration);
    }
}
