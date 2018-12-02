using System.Collections.Generic;

namespace JDBot.Infrastructure.Texts
{
    internal class RegexesInfo
    {
        public IList<string> DefaultOptions { get; set; }
        public IList<RegexInfo> Regexes { get; set; }
    }
}