using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Principal;
using VindiBank.Business.Interfaces;
using VindiBank.Models;
using VindiBank.Models.API;
using VindiBank.Models.Enum;
using static System.Net.Mime.MediaTypeNames;

namespace VindiBank.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class HomeController : ControllerBase
    {

        private readonly IFluxoAPIControll _fluxoAPIcontroll;
        private readonly IConfiguration _configuration;
        public HomeController(IFluxoAPIControll fluxoAPIControll, IConfiguration configuration)
        {
            _fluxoAPIcontroll = fluxoAPIControll;
            _configuration = configuration;
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(AccountVindi accountVindi)
        {
            try
            {
                await _fluxoAPIcontroll.ProcessaDados(MetodoAPI.createAccount, accountVindi, _configuration);
                return Ok(200 + " - Nova Conta Incluida, Sucesso!");
            }
            catch (Exception ex)
            {
                return NotFound(400 + " - " + ex.Message);
            }
            
        }
        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts(string documento = "",string nomeCliente = "")
        {
            try
            {
                var searchAccount = new SearchAccount
                {
                    documento = documento,
                    nomeCliente = nomeCliente
                };

                var conta = await _fluxoAPIcontroll.ProcessaDados(MetodoAPI.getAccounts, searchAccount, _configuration);
                return Ok(conta);
            }
            catch (Exception ex)
            {
                return NotFound(400 + " - " + ex.Message);
            }
        }

        [HttpDelete("AccountDeactivation")]
        public async Task<IActionResult> AccountDeactivation(DeleteAccount deleteAccount)
        {
            try
            {
                await _fluxoAPIcontroll.ProcessaDados(MetodoAPI.accountDeactivation, deleteAccount, _configuration);
                return Ok(200 + " - Conta Deletada, Sucesso!");
            }
            catch (Exception ex)
            {
                return NotFound(400 + " - " + ex.Message);
            }
        }

        [HttpPut("AccountTransfer")]
        public async Task<IActionResult> AccountTransfer(TransferBetweenAccounts transferBetweenAccounts)
        {
            try
            {
                var transferencia =  await _fluxoAPIcontroll.ProcessaDados(MetodoAPI.accountTransfer, transferBetweenAccounts, _configuration);

                if (transferencia is string s && s.StartsWith("Erro:", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(s);
                }

                return Ok(200 + " - Transferencia Realizada, Sucesso!");
            }
            catch (Exception ex)
            {
                return NotFound(400 + " - " + ex.Message);
            }
        }

    }
}
