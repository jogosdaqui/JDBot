using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp
{
    [Command]
    public abstract class CommandBase
    {
        [Option("--v", Description = "Define o nível da verbosidade (debug, info, warn ou error")]
        public LogVerbosity Verbosity { get; set; } = LogVerbosity.Info;

        protected async virtual Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            Logger.Initialize(new ConsoleLogger(), Verbosity);
   
            return await Task.Run(() => 0);
        }

    }
}
