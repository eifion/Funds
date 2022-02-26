using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LibFunds;

public class Incomings
{
    private readonly string _dbConnection;

    public Incomings(string dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public List<string> GetRecentIncomings()
    {
        using IDbConnection conn = new SqliteConnection(_dbConnection);
        var incomings = conn.Query<IncomingsModel>("SELECT title, start_date AS StartDate, end_date AS EndDate, amount FROM incomings ORDER BY start_date DESC LIMIT 10;");
        return incomings.Select(i => i.ToString()).ToList();
    }

    public record IncomingsModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Amount { get; set; }

        public override string ToString() => $"{StartDate:yyyy-MM-dd} {EndDate:yyyy-MM-dd} {Title} {Amount / 100.0m:C2}";
    }
}