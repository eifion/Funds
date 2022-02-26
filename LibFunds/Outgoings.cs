using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LibFunds;

public class Outgoings
{
    private readonly string _dbConnection;

    public Outgoings(string dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public string AddOutgoing(string title, decimal amount, string dateValue, string fundIdentifier)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title cannot be blank";
        }

        if (amount < 0.0m)
        {
            return "Amount cannot be negative";
        }

        DateOnly date;
        if (string.IsNullOrWhiteSpace(dateValue) || string.Equals(dateValue, "today", StringComparison.InvariantCultureIgnoreCase))
        {
            date = DateOnly.FromDateTime(DateTime.Today);
        }
        else if (string.Equals(dateValue, "yesterday", StringComparison.InvariantCultureIgnoreCase))
        {
            date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        }
        else if (!DateOnly.TryParseExact(dateValue, "yyyy-MM-dd", out date))
        {
            return "Date supplied is not valid. Should be in format yyyy-MM-dd";
        }

        var funds = new Funds(_dbConnection);
        // If no fundId supplied so assume the default fund.
        string? identifier = string.IsNullOrWhiteSpace(fundIdentifier) ? funds.GetDefaultFund() : funds.GetFundByIdentifier(fundIdentifier);

        // If no fund found bail out.
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return $"Cannot find fund {fundIdentifier}";
        }

        using IDbConnection conn = new SqliteConnection(_dbConnection);
        try
        {
            var model = new NewOutgoingModel(title, amount, date, identifier);
            conn.Query("INSERT INTO Outgoings(title, amount, date, fund_identifier) VALUES(@Title, @Amount, @Date, @FundIdentifier);", model);
            return string.Empty;
        }
        catch (Exception ex)
        {
            return $"An error occurred while adding the outgoing: {ex.Message}";
        }
    }

    public List<string> GetRecentOutgoings()
    {
        using IDbConnection conn = new SqliteConnection(_dbConnection);
        var outgoings = conn.Query<OutgoingsModel>("SELECT Date, Title, Amount FROM Outgoings ORDER BY Date DESC LIMIT 10;");
        return outgoings.Select(o => o.ToString()).ToList();
    }
}

public record NewOutgoingModel
{
    public string Title { get; init; }
    public int Amount { get; init; }
    public string Date { get; init; }
    public string FundIdentifier { get; init; }

    public NewOutgoingModel(string title, decimal amount, DateOnly date, string fundIdentifier)
    {
        Title = title;
        Amount = (int)(amount * 100.0m);
        Date = date.ToString("yyyy-MM-dd");
        FundIdentifier = fundIdentifier;
    }
}

public record OutgoingsModel
{
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Amount { get; set; }

    public override string ToString() => $"{Date:yyyy-MM-dd} {Title} {Amount / 100.0m:C2}";
}