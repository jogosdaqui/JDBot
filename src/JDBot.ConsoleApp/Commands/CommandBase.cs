using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands
{
    [Command]
    public abstract class CommandBase
    {
        public int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return 1;
        }
    }
}
