using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "post", Description = "Comandos relacionados a posts.")]
    [Subcommand("new", typeof(NewSubcommand))]
    [Subcommand("rename", typeof(RenameSubcommand))]
    [Subcommand("grab", typeof(GrabSubcommand))]
    public class PostCommand : CommandBase
    {

    }
}
