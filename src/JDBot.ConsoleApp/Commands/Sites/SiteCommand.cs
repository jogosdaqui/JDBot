using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Sites
{
    [Command(Name = "site", Description = "Realiza operações em relação a todo site, como publicá-lo.")]
    [Subcommand("publish", typeof(PublishSubcommand))]
    [Subcommand("release", typeof(ReleaseSubcommand))]
    public class SiteCommand : CommandBase
    {
    }
}
