using Funds.Commands;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
namespace Funds;

public class Program
{
    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddUserSecrets<Program>()
                        .Build();
        var dbConn = config.GetConnectionString("DefaultConnection");

        var fundsCommands = new FundsCommands(dbConn);
        var rootCommand = fundsCommands.Total();

        var outgoingsCommands = new OutgoingsCommands(dbConn);
        var listOutgoingsCommand = outgoingsCommands.List();
        listOutgoingsCommand.AddCommand(outgoingsCommands.Add());
        rootCommand.AddCommand(listOutgoingsCommand);

        rootCommand.Invoke(args);
    }
}
