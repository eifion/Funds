using System.CommandLine;
using LibFunds;

namespace Funds.Commands
{
    public class OutgoingsCommands
    {
        private readonly string _dbConnection;

        public OutgoingsCommands(string dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Command List()
        {
            var listCommand = new Command("outgoings", "Commands related to outgoings. By default will list the ten most recent outgoings.");
            listCommand.SetHandler(() => new Outgoings(_dbConnection).GetRecentOutgoings().ForEach((outgoing) => Console.WriteLine(outgoing)));
            return listCommand;
        }

        public Command Add()
        {
            var addCommand = new Command("add", "Add an outgoing.");

            var nameOption = new Option<string>("--name")
            {
                Description = "A name for the outgoing",
                IsRequired = true,
                Arity = ArgumentArity.ExactlyOne
            };
            nameOption.AddAlias("-n");

            var amountOption = new Option<decimal>("--amount")
            {
                Description = "The amount",
                IsRequired = true,
                Arity = ArgumentArity.ExactlyOne
            };
            amountOption.AddAlias("-a");

            var dateOption = new Option<string>("--date")
            {
                Description = "The date the outgoing should be added for (defaults to the current day).",
                Arity = ArgumentArity.ZeroOrOne
            };
            dateOption.AddAlias("-d");

            var fundOption = new Option<string>("--fund")
            {
                Description = "The fund the outgoing should be added to (defaults to the default fund).",
                Arity = ArgumentArity.ZeroOrOne
            };
            fundOption.AddAlias("-f");

            addCommand.AddOption(nameOption);
            addCommand.AddOption(amountOption);
            addCommand.AddOption(dateOption);
            addCommand.AddOption(fundOption);

            addCommand.SetHandler((string name, decimal amount, string date, string fundId) =>
            {
                var response = new Outgoings(_dbConnection).AddOutgoing(name, amount, date, fundId);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    Console.WriteLine(response);
                }
            },
            nameOption, amountOption, dateOption, fundOption);

            return addCommand;
        }
    }
}