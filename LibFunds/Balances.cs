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
        var balances = conn.Query<int>(@"
         SELECT Amount * CAST(julianday(@Today) - julianday(start_date) + 1 AS INTEGER) / CAST(julianday(end_date) - julianday(start_date) + 1 AS INTEGER)
         FROM Incomings WHERE end_date > @Today
         UNION ALL SELECT SUM(Amount) FROM Incomings WHERE end_date <= @Today
         UNION ALL SELECT -1 * SUM(amount) FROM Outgoings WHERE date <= @Today
         UNION ALL SELECT SUM(opening_balance) FROM Funds", parameters);
        return balances.Sum() / 100.0m;
    }
}