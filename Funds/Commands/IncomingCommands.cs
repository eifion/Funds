using System.CommandLine;
using LibFunds;

namespace Funds.Commands
{
    public class IncomingsCommands
    {
        private readonly string _dbConnection;

        public IncomingsCommands(string dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Command List()
        {
            var listCommand = new Command("incomings", "Commands related to incomings. By default will list the ten most recent incomings.");
            listCommand.SetHandler(() => new Incomings(_dbConnection).GetRecentIncomings().ForEach((incoming) => Console.WriteLine(incoming)));
            return listCommand;
        }
    }
}