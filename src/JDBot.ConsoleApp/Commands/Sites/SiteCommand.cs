using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Sites
{
    [Command(Name = "site", Description = "Realiza operações em relação a todo site, como publicá-lo.")]
    [Subcommand("publish", typeof(PublishSubcommand))]
    public class SiteCommand : CommandBase
    {

    }
}
