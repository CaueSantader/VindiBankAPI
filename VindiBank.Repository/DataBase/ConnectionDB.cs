using Npgsql;
using System.Data;
using VindiBank.Models.DataBase;
using VindiBank.Repository.Interfaces;

namespace VindiBank.Repository.DataBase
{
    public class ConnectionDB : IConnectionDB
    {
        private IDbConnection dbConnection;

        public IDbConnection ConexaoPostgres(string connString)        
        {

            return new NpgsqlConnection(connString);
        }
    }
}
