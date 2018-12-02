using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JDBot.Domain.Sites;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Net;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp
{
    [Command(Name = "site", Description = "Realiza operações em relação a todo site, como publicá-lo.")]
    public class SiteCommand : CommandBase
    {
        [Required]
        [Option("--k", Description = "A API Key para o AppVeyor.")]
        public string ApiKey { get; set; }

        [Option("--publish", Description = "Publica o site com o que último commit no master.")]
        public bool Publish { get; set; }

        [Option("--publish-status", Description = "Obtém o status atual da publicação do site.")]
        public bool PublishStatus { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            var publisher = new AppVeyorSitePublisher(ApiKey);
            var site = new Site(publisher);

            if (Publish)
            {
                Logger.Info("Iniciando a publicação...");
                await site.PublishAsync();
                Logger.Info($"Publicação agendada no AppVeyor. Em minutos o site estará atualizado.");
                return 0;
            }
            else if (PublishStatus)
            {
                Logger.Info("Consultando o status...");
                var status = await site.GetLatestPublicationStatus();
                Logger.Info($"Status: {status}");
                return 0;
            }

            return 1;
        }
    }
}
