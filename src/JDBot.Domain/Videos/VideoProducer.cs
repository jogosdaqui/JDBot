using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Videos
{
    public class VideoProducer
    {
        private readonly IPostExtractor _postExtractor;
        private readonly IResourceClient _resourceClient;
        private readonly IVideoBuilder _builder;

        public VideoProducer(IVideoBuilder builder, IPostExtractor postExtractor, IResourceClient resourceClient)
        {
            _builder = builder;
            _postExtractor = postExtractor;
            _resourceClient = resourceClient;
        }

        public async Task<VideoProducerOutput> MakeVideoAsync(IEnumerable<string> postsUrls, VideoProducerOptions options)
        {
            var posts = new List<Post>();
            var duration = options.DurationPerUrl;

            foreach (var url in postsUrls)
            {
                var post = await _postExtractor.ExtractAsync(url);
                var logo = await _resourceClient.DownloadImageAsync(post.Logo);
                _builder
                    .AddImage(logo, duration)
                    .AddTitle(post.Title,duration);
                    
                if(post.Companies.Count > 0)
                {
                    _builder.AddDescription(post.Companies[0], duration);
                }

            }

            return new VideoProducerOutput(_builder.Build());
        }
    }
}
