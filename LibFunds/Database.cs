using System.Data;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;

namespace LibFunds;

public static class Database
{
    public static bool Create(string path)
    {
        var dbConnection = $"Data Source={path}";
        try
        {
            using IDbConnection conn = new SqliteConnection(dbConnection);
            conn.Execute("CREATE TABLE Funds ( identifier TEXT, name TEXT, start_date TEXT, opening_balance INTEGER, is_default INTEGER, is_active INTEGER, PRIMARY KEY(identifier) )");
            conn.Execute("CREATE TABLE incomings ( id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT NOT NULL, start_date TEXT NOT NULL, end_date TEXT NOT NULL, amount INTEGER NOT NULL, fund_identifier TEXT NOT NULL, FOREIGN KEY(fund_identifier) REFERENCES Funds(identifier))");
            conn.Execute("CREATE TABLE outgoings ( id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT NOT NULL, amount INTEGER NOT NULL, date TEXT NOT NULL, fund_identifier TEXT NOT NULL, FOREIGN KEY(fund_identifier) REFERENCES Funds(identifier))");
            conn.Execute("CREATE TABLE transfers  ( incoming_id  INTEGER NOT NULL, outgoing_id  INTEGER NOT NULL, CONSTRAINT PK_transfers  PRIMARY KEY ( incoming_id , outgoing_id ), CONSTRAINT FK_transfers_incomings_incoming_id  FOREIGN KEY ( incoming_id ) REFERENCES incomings  ( id ) ON DELETE CASCADE, CONSTRAINT FK_transfers_outgoings_outgoing_od  FOREIGN KEY ( outgoing_id ) REFERENCES outgoings  ( id ) ON DELETE CASCADE )");
            conn.Execute(@"
              CREATE VIEW transactions AS 
              	SELECT i.Id,  'incoming' AS TransactionType, i.title, i.amount, i.start_date, i.end_date, f.identifier AS fund_identifier, f.name AS fund_name, NULL AS transfer_id, NULL AS transfer_fund_name
              	FROM incomings i
              	INNER JOIN funds f ON f.identifier = i.fund_identifier
              	LEFT JOIN transfers t ON t.incoming_id = i.Id
              	LEFT JOIN outgoings o ON t.outgoing_id = o.Id
              	WHERE o.id IS NULL
              	UNION
              	SELECT o.Id, CASE WHEN t.incoming_id IS NULL THEN 'outgoing' ELSE 'transfer' END AS transaction_type, o.title, o.amount, o.[date] AS start_date, o.[Date] AS end_date, f.identifier AS fund_identifier, f.name AS fund_name, t.incoming_id AS transfer_id, f2.Name AS transfer_fund_name
              	FROM outgoings o
             	INNER JOIN funds f ON f.identifier = o.fund_identifier
              	LEFT JOIN transfers t ON t.outgoing_id = o.id
              	LEFT JOIN incomings i ON t.incoming_id = i.id
              	LEFT JOIN funds f2 ON i.fund_identifier = f2.identifier;");
            return true;
        }
        catch
        {
            Console.WriteLine("Error creating database");
            return false;
        }
    }
}