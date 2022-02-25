using System.CommandLine;
using LibFunds;

namespace Funds.Commands
{
    public class FundsCommands
    {
        private readonly string _dbConnection;

        public FundsCommands(string dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Command Total()
        {
            var totalCommand = new RootCommand("Displays the current total of all funds");
            totalCommand.SetHandler(() =>
            {
                var balance = new Balances(_dbConnection).GetOverallBalance();
                Console.WriteLine($"{balance:C2}");
            });

            return totalCommand;
        }
    }
}