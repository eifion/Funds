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

        public Command Add()
        {
            var addCommand = new Command("add", "Add an incoming.");

            var nameOption = new Option<string>("--name")
            {
                Description = "A name for the incoming",
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

            var startDateOption = new Option<string>("--start-date")
            {
                Description = "The start date the incoming should be added for (defaults to the current day).",
                Arity = ArgumentArity.ZeroOrOne
            };
            startDateOption.AddAlias("-s");

            var endDateOption = new Option<string>("--end-date")
            {
                Description = "The end date the incoming should be added for (defaults to the current day).",
                Arity = ArgumentArity.ZeroOrOne
            };
            endDateOption.AddAlias("-e");

            var fundOption = new Option<string>("--fund")
            {
                Description = "The fund the incoming should be added to (defaults to the default fund).",
                Arity = ArgumentArity.ZeroOrOne
            };
            fundOption.AddAlias("-f");

            addCommand.AddOption(nameOption);
            addCommand.AddOption(amountOption);
            addCommand.AddOption(startDateOption);
            addCommand.AddOption(endDateOption);
            addCommand.AddOption(fundOption);

            addCommand.SetHandler((string name, decimal amount, string startDate, string endDate, string fundIdentifier) =>
            {
                var response = new Incomings(_dbConnection).Add(name, amount, startDate, endDate, fundIdentifier);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    Console.WriteLine(response);
                }
            }, nameOption, amountOption, startDateOption, endDateOption, fundOption);

            return addCommand;
        }
    }
}