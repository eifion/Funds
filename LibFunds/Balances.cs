using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LibFunds;
public class Balances
{
    private readonly string _dbConnection;

    public Balances(string dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public decimal GetOverallBalance()
    {
        using IDbConnection conn = new SqliteConnection(_dbConnection);
        var parameters = new { DateTime.Today };
        //         var balances = conn.Query<int>(@"
        // SELECT Amount * CAST(julianday(@Today) - julianday(StartDate) + 1 AS INTEGER) / CAST(julianday(EndDate) - julianday(StartDate) + 1 AS INTEGER)
        // FROM Incomings WHERE EndDate > @Today
        // UNION ALL SELECT SUM(Amount) FROM Incomings WHERE EndDate <= @Today
        // UNION ALL SELECT -1 * SUM(Amount) FROM Outgoings
        // UNION ALL SELECT SUM(OpeningBalance) FROM Funds", parameters);
        var balances = conn.Query<int>("SELECT SUM(opening_balance) FROM Funds");
        return balances.Sum() / 100.0m;
    }
}