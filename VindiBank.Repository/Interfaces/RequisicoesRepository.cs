using Dapper;
using Npgsql;
using System.Text.Json;
using VindiBank.Models.API;
using VindiBank.Models.DataBase;
using VindiBank.Repository.Repositories;

namespace VindiBank.Repository.Interfaces
{
    public class RequisicoesRepository : IRequisicoesRepository
    {
        private readonly IConnectionDB _connectionDB;

        public RequisicoesRepository(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void IncludeNewClients(string connString, object oEntrada)
        {
            var conta = (AccountVindi)oEntrada;
            
            using var conn = _connectionDB.ConexaoPostgres(connString);
            {
                conn.Open();
                Console.WriteLine("Conectado!");
                using var cmd = new NpgsqlCommand(@$"INSERT INTO es_vindiclientes_table (nome_cliente_str,documento_str,tipo_conta_str,saldo_dec,data_abertura_conta_tstamp,status_conta) 
                                                    VALUES('{conta.nomeCliente}','{conta.documento}','{conta.PersonType}',{conta.saldo},current_timestamp,'True')", (NpgsqlConnection?)conn);
                using var reader = cmd.ExecuteReader();
                conn.Close();
            }

        }

        public AccountVindi? SearchClients(string connString, object oEntrada)
        {
            var conta = (SearchAccount)oEntrada;
            string sql = "";
            using var conn = _connectionDB.ConexaoPostgres(connString);
            {
                conn.Open();
                if (conta.nomeCliente != "")
                {
                     sql = @$"SELECT id_conta_vindi_int AS IdConta,
                                    nome_cliente_str AS NomeCliente,
                                    documento_str AS Documento,
                                    tipo_conta_str AS PersonType,
                                    saldo_dec AS Saldo,
                                    data_abertura_conta_tstamp AS DataAberturaConta,
                                    status_conta AS StatusConta
                                                FROM es_vindiclientes_table 
                                                WHERE status_conta = 'True' 
                                    AND nome_cliente_str ILIKE '%{conta.nomeCliente}%' 
                                    ";
                }
                else
                { 
                sql = @$"SELECT id_conta_vindi_int AS IdConta,
                                    nome_cliente_str AS NomeCliente,
                                    documento_str AS Documento,
                                    tipo_conta_str AS PersonType,
                                    saldo_dec AS Saldo,
                                    data_abertura_conta_tstamp AS DataAberturaConta,
                                    status_conta AS StatusConta
                                                FROM es_vindiclientes_table 
                                                WHERE status_conta = 'True' 
                                    AND documento_str = '{conta.documento}'
                                    ";
                }
                var contas = conn.QueryFirstOrDefault<AccountVindi>(sql);

                return contas;
            }
        }

        public void DisablesClients(string connString, object oEntrada)
        {
            var conta = (DeleteAccount)oEntrada;
            using var conn = _connectionDB.ConexaoPostgres(connString);
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(@$"UPDATE es_vindiclientes_table SET status_conta = 'False', data_encerramento_conta_tstamp = current_timestamp WHERE documento_str = '{conta.documento}'", (NpgsqlConnection?)conn);
                using var reader = cmd.ExecuteReader();
                conn.Close();
            }

        }

        public string TransitionClientes(string connString, object oEntrada)
        {
            var conta = (TransferBetweenAccounts)oEntrada;

            if (conta == null)
                return "Erro: Dados de transferência inválidos.";

            if (string.IsNullOrWhiteSpace(conta.documentoSaida) || string.IsNullOrWhiteSpace(conta.documentoEntrada))
                return "Erro: Documento de origem ou destino não informado.";

            if (conta.saldoDocumentoSaida <= 0)
                return "Erro: O valor da transferência deve ser maior que zero.";

            using var conn = _connectionDB.ConexaoPostgres(connString);
            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                const string selectSql = @"
                    SELECT id_conta_vindi_int AS Id,
                           documento_str AS Documento,
                           saldo_dec AS Balance,
                           CASE WHEN status_conta = 'True' THEN TRUE ELSE FALSE END AS IsActive
                    FROM es_vindiclientes_table
                    WHERE documento_str = @docSaida OR documento_str = @docEntrada
                    FOR UPDATE;";

                var contas = conn.Query<AccountRow>(
                    selectSql,
                    new { docSaida = conta.documentoSaida, docEntrada = conta.documentoEntrada },
                    tx
                ).ToList();

                var origem = contas.FirstOrDefault(c => c.Documento == conta.documentoSaida);
                var destino = contas.FirstOrDefault(c => c.Documento == conta.documentoEntrada);

                if (origem == null)
                    return "Erro: Conta de origem não encontrada.";

                if (destino == null)
                    return "Erro: Conta de destino não encontrada.";

                if (!origem.IsActive || !destino.IsActive)
                    return "Erro: Uma ou ambas as contas estão inativas.";

                if (origem.Id == destino.Id)
                    return "Erro: Conta de origem e destino não podem ser a mesma.";

                if (origem.Balance < conta.saldoDocumentoSaida)
                    return "Erro: Saldo insuficiente na conta de origem.";

                const string debitarSql = @"UPDATE es_vindiclientes_table
                                            SET saldo_dec = saldo_dec - @valor
                                            WHERE id_conta_vindi_int = @id;";

                const string creditarSql = @"UPDATE es_vindiclientes_table
                                             SET saldo_dec = saldo_dec + @valor
                                             WHERE id_conta_vindi_int = @id;";

                var debited = conn.Execute(debitarSql, new { valor = conta.saldoDocumentoSaida, id = origem.Id }, tx);
                var credited = conn.Execute(creditarSql, new { valor = conta.saldoDocumentoSaida, id = destino.Id }, tx);

                if (debited != 1 || credited != 1)
                {
                    tx.Rollback();
                    return "Falha ao atualizar saldos das contas.";
                }

                tx.Commit();
                return $"Transferência realizada com sucesso! Valor: {conta.saldoDocumentoSaida:C}";
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return $"Erro: durante a transferência: {ex.Message}";
            }

        }

        public class AccountRow
        {
            public long Id { get; set; }
            public string Documento { get; set; } = string.Empty;
            public decimal Balance { get; set; }
            public bool IsActive { get; set; }
        }

    }
}
