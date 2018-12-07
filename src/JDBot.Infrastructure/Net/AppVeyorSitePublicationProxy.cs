using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JDBot.Domain.Sites;

namespace JDBot.Infrastructure.Net
{
    public class AppVeyorSitePublicationProxy : ISitePublicationProxy
    {
        private static readonly string _apiUrl = "https://ci.appveyor.com/api/";
        private readonly string _apiKey;

        public AppVeyorSitePublicationProxy(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<PublicationStatus> GetPublicationStatusAsync()
        {
            var response = await GetAsync("projects/giacomelli/jogosdaqui-github-io-jekyll/branch/master");

            switch (response.Build.Status)
            {
                case "pending":
                    return PublicationStatus.NotStarted;

                case "queued":
                    return PublicationStatus.Queued;
             
                 case "running":
                    return PublicationStatus.Running;

                case "success":
                    return PublicationStatus.Success;

                default:
                    return PublicationStatus.Failed;
            }
        }

        public async Task PublishAsync()
        {
            await PostAsync("builds", new AppVeyorRequest { Branch = "master" });
        }

        private async Task<AppVeyorResponse> GetAsync(string route)
        {
            using (var client = CreateClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                using (var response = await client.GetAsync($"{_apiUrl}{route}"))
                {
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsAsync<AppVeyorResponse>();
                }
            }
        }

        private async Task PostAsync(string route, AppVeyorRequest request)
        {
            using (var client = CreateClient())
            {
                var content = new Dictionary<string, string>
                {
                    { "accountName", request.AccountName },
                    { "projectSlug", request.ProjectSlug },
                    { "branch", request.Branch }
                };

                var message = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}{route}") { Content = new FormUrlEncodedContent(content) };

                using (var response = await client.SendAsync(message))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            return client;
        }
    }
}
