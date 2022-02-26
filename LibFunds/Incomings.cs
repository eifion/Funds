using Dapper;
using LibFunds.Extensions;
using Microsoft.Data.Sqlite;
using System.Data;

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

    public string Add(string title, decimal amount, string startDateValue, string endDateValue, string fundIdentifierValue)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title cannot be blank";
        }

        if (amount < 0.0m)
        {
            return "Amount cannot be negative";
        }

        if (!startDateValue.TryParseDate(out DateOnly startDate))
        {
            return "Start date supplied is not valid. Should be in the format yyyy-MM-dd";
        }

        if (!endDateValue.TryParseDate(out DateOnly endDate))
        {
            return "Start date supplied is not valid. Should be in the format yyyy-MM-dd";
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
            var newIncoming = new NewIncomingModel(title, amount, startDate, endDate, fundIdentifier);
            conn.Query(
                @"INSERT INTO Incomings(title, amount, start_date, end_date, fund_identifier)
                  VALUES(@Title, @Amount, @StartDate, @EndDate, @FundIdentifier);", newIncoming);
            return string.Empty;
        }
        catch (Exception ex)
        {
            return $"An error occurred while adding the incoming: {ex.Message}";
        }
    }

    public record NewIncomingModel
    {
        public string Title { get; init; }

        public int Amount { get; init; }

        public string StartDate { get; init; }

        public string EndDate { get; init; }

        public string FundIdentifier { get; init; }

        public NewIncomingModel(string title, decimal amount, DateOnly startDate, DateOnly endDate, string fundIdentifier)
        {
            Title = title;
            Amount = (int)(amount * 100.0m);
            StartDate = startDate.ToString("yyyy-MM-dd");
            EndDate = endDate.ToString("yyyy-MM-dd");
            FundIdentifier = fundIdentifier;
        }
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