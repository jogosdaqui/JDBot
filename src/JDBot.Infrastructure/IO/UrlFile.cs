using System.Collections.Generic;

namespace JDBot.Infrastructure.IO
{
    public class UrlFile
    {
        public string JekyllRootFolder { get; set; }
        public IEnumerable<UrlFileItem> Items { get; set; }
    }
}
