using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands
{
    [Command]
    public abstract class SubCommandBase
    {
        [Option("--v|--verbosity", Description = "Define o nível da verbosidade (debug, info, warn ou error")]
        [AllowedValues("debug", "info", "warn", "error", IgnoreCase = true)]
        public LogVerbosity Verbosity { get; set; } = LogVerbosity.Info;

        protected async virtual Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            Logger.Initialize(new ConsoleLogger(), Verbosity);
   
            return await Task.Run(() => 0);
        }

    }
}
