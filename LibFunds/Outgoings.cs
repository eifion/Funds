using Dapper;
using LibFunds.Extensions;
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

    public string AddOutgoing(string title, decimal amount, string dateValue, string fundIdentifierValue)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title cannot be blank";
        }

        if (amount < 0.0m)
        {
            return "Amount cannot be negative";
        }

        if (!dateValue.TryParseDate(out DateOnly date))
        {
            return "Date supplied is not valid. Should be in format yyyy-MM-dd";
        }

        var funds = new Funds(_dbConnection);
        // If no fundId supplied so assume the default fund.
        string? fundIdentifier = string.IsNullOrWhiteSpace(fundIdentifierValue) ? funds.GetDefaultFund() : funds.GetFundByIdentifier(fundIdentifierValue);

        // If no fund found bail out.
        if (string.IsNullOrWhiteSpace(fundIdentifier))
        {
            return $"Cannot find fund {fundIdentifierValue}";
        }

        using IDbConnection conn = new SqliteConnection(_dbConnection);
        try
        {
            var newOutgoing = new NewOutgoingModel(title, amount, date, fundIdentifier);
            conn.Query(@"INSERT INTO Outgoings(title, amount, date, fund_identifier) 
                         VALUES(@Title, @Amount, @Date, @FundIdentifier);", newOutgoing);
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