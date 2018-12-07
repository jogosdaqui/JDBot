using System.Text.RegularExpressions;

namespace JDBot.Infrastructure.Framework
{
    public class SemanticVersioning
    {
        private static readonly Regex _getTagVersionRegex = new Regex(@"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)", RegexOptions.Compiled);
       
        public SemanticVersioning(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public static SemanticVersioning Parse(string value)
        {
            var match = _getTagVersionRegex.Match(value);

            if (match.Success)
            {
                return new SemanticVersioning(
                    int.Parse(match.Groups["major"].Value),
                    int.Parse(match.Groups["minor"].Value),
                    int.Parse(match.Groups["patch"].Value)
                );
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
