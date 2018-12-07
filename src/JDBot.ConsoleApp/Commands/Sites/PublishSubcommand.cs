using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JDBot.Domain.Sites;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Net;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Sites
{
    [Command(Name = "publish", Description = "Realiza operações em relação a publicação.")]
    public class PublishSubcommand : SubCommandBase
    {
        [Option("--apikey", Description = "A API Key para o AppVeyor. Pode ser definida diretamente no comando ou através da variável de ambiente JDBOT_APPVEYOR_APIKEY")]
        public string ApiKey { get; set; } = Environment.GetEnvironmentVariable("JDBOT_APPVEYOR_APIKEY");

        [Option("--update", Description = "Atualiza o site com o que último commit no master.")]
        public bool Update { get; set; }

        [Option("--status", Description = "Obtém o status atual da publicação do site.")]
        public bool Status { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            if (String.IsNullOrEmpty(ApiKey))
            {
                app.ShowHelp();
                return 1;
            }

            var proxy = new AppVeyorSitePublicationProxy(ApiKey);
            var publisher = new SitePublisher(proxy);

            if (Update)
            {
                Logger.Info("Iniciando a publicação...");
                await publisher.PublishAsync();
                Logger.Info($"Publicação agendada no AppVeyor. Em minutos o site estará atualizado.");
                return 0;
            }
            else if (Status)
            {
                Logger.Info("Consultando o status...");
                var status = await publisher.GetLatestPublicationStatus();
                Logger.Info($"Status: {status}");
                return 0;
            }

            return 1;
        }
    }
}
