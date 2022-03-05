using System.CommandLine;
using LibFunds;

namespace Funds.Commands;

public class DatabaseCommands
{
    public static Command Show()
    {
        var showCommand = new Command("database", "Commands related to the database");
        showCommand.SetHandler(() => Console.WriteLine("TODO: Show path to DB"));
        return showCommand;
    }

    public static Command Create()
    {
        var createCommand = new Command("create", "Create a Funds database");

        var pathArgument = new Argument<string>("path")
        {
            Description = "A path to where the datbase should be stored.",
            Arity = ArgumentArity.ExactlyOne
        };

        createCommand.AddArgument(pathArgument);
        createCommand.SetHandler((string path) =>
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                Console.WriteLine($"Cannot create a database at {fullPath}. The file already exists");
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Console.WriteLine($"Cannot create a database at {fullPath}. The directory does not exist");
                return;
            }

            if (File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                Console.WriteLine($"Cannot create a database at {fullPath}. The path specified is a directory.");
                return;
            }

            try
            {
                File.Create(fullPath).Close();
                if (!Database.Create(fullPath))
                {
                    Console.WriteLine($"Could not create a database at {fullPath}");
                    return;
                }
            }
            catch
            {
                Console.WriteLine($"Could not create a file at {fullPath}");
                return;
            }
        }, pathArgument);

        return createCommand;
    }
}