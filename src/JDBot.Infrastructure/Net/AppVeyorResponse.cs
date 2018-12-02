namespace JDBot.Infrastructure.Net
{
    public class AppVeyorResponse
    {
        public AppVeyorBuild Build { get; set; }
    }

    public class AppVeyorBuild
    {
        public string Status { get; set; }
    }
}
