using System;
using JDBot.ConsoleApp.Commands.Posts;
using JDBot.ConsoleApp.Commands.Sites;
using JDBot.Infrastructure.Framework;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp
{
    [Subcommand("post", typeof(PostCommand))]
    [Subcommand("site", typeof(SiteCommand))]
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CommandLineApplication.Execute<Program>(args);
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();

            return 1;
        }
    }
}
