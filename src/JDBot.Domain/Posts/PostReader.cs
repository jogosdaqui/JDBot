using System.Collections.Generic;
using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Posts
{
    public class PostReader
    {
        private readonly IEnumerable<IPostExtractor> _extractors;

        public PostReader(IEnumerable<IPostExtractor> extractors)
        {
            _extractors = extractors;
        }

        public async Task<Post> ReadAsync(string url)
        {
            Logger.Info($"Lendo a url {url}...");

            foreach (var extractor in _extractors)
            {
                var post = await extractor.ExtractAsync(url);

                if (post != null)
                {
                    Logger.Debug($"Utilizando o extrator para post {extractor.GetType().Name.Replace("PostExtractor", "")}");
                    return post;
                }
            }

            Logger.Error("Não existe extrator disponível para esse tipo de post");
            return null;
        }
    }
}
