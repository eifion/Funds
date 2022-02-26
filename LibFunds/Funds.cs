using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LibFunds
{
    public class Funds
    {
        private readonly string _dbConnection;

        public Funds(string dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public string? GetDefaultFund()
        {
            using IDbConnection conn = new SqliteConnection(_dbConnection);
            return conn.QueryFirstOrDefault<string>("SELECT identifier FROM Funds WHERE is_default = true;");
        }

        public string? GetFundByIdentifier(string fundIdentifier)
        {
            using IDbConnection conn = new SqliteConnection(_dbConnection);
            var parameters = new { Identifier = fundIdentifier };
            return conn.QueryFirstOrDefault<string>("SELECT identifier FROM Funds WHERE identifier = @identifier;", parameters);
        }
    }
}