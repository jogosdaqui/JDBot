namespace JDBot.Infrastructure.Net
{
    public class AppVeyorRequest
    {
        public string AccountName { get; } = "giacomelli";
        public string ProjectSlug { get; } = "jogosdaqui-github-io-jekyll";
        public string Branch { get; set; } = "master";
    }
}
