using System;
using System.Linq;
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
                new GenericPostExtractor()
            };

            _reader = new PostReader(extractors);

            // Escritor.
            var resourceClient = new WebResourceClient();
            var fs = new FileSystem();
            _writer = new PostWriter(jekyllRootFolder, resourceClient, fs);
        }

        public async Task WritePostAsync(string sourcePostUrl, PostConfig config)
        {
            var post = await _reader.ReadAsync(sourcePostUrl);

            if(post != null)
                await _writer.WriteAsync(post, config);
        }
    }
}
