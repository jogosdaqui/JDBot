using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp
{
    [Subcommand("post", typeof(PostCommand))]
    [Subcommand("site", typeof(SiteCommand))]
    class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();

            return 1;
        }
    }
}
