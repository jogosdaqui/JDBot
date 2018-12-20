using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using JDBot.Infrastructure.IO;
using JDBot.Infrastructure.Net;

namespace JDBot.Application
{
    public class PostService
    {
        private readonly string _jekyllRootFolder;
        private readonly PostReader _reader;
        private readonly PostWriter _writer;

        public PostService(string jekyllRootFolder)
        {
            _jekyllRootFolder = jekyllRootFolder;

            // Leitor.
            var extractors = new IPostExtractor[]
            {
                new WordPressPostExtractor(),
                new DoPresskitVariant1PostExtractor(),
                new DoPresskitVariant2PostExtractor(),
                new GooglePlayPostExtractor(),
                new SteamPostExtractor(),
                new GenericPostExtractor()
            };

            _reader = new PostReader(extractors);

            // Escritor.
            var resourceClient = new WebResourceClient();
            var fs = new FileSystem();
            _writer = new PostWriter(jekyllRootFolder, resourceClient, fs);
        }

        public async Task<PostInfo> WritePostAsync(string sourcePostUrl, PostConfig config)
        {
            var post = await _reader.ReadAsync(sourcePostUrl);

            if(post != null)
                return await _writer.WriteAsync(post, config);

            return PostInfo.Empty;
        }

        public async Task<PostInfo> WritePostAsync(Post post)
        {
            return await _writer.WriteAsync(post, PostConfig.Empty);
        }

        public async Task<PostInfo> RenamePostAsync(PostInfo oldPost, PostInfo newPost)
        {
            return await _writer.RenameAsync(oldPost, newPost);
        }
    }
}
