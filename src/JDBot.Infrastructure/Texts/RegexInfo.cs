using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JDBot.Infrastructure.Texts
{
    internal class RegexInfo
    {
        public string Pattern { get; set; }
        public IList<string> Options { get; set; }
        public IList<string> Tags { get; set; }
        public string Replacement { get; set; }
        public IList<string> Responses { get; set; }
        public Regex Regex { get; private set; }

        public void Initialize(RegexOptions defaultOptions)
        {
            Options = Options ?? new List<string>();
            Tags = Tags ?? new List<string>();
            Responses = Responses ?? new List<string>();

            try
            {
                Regex = new Regex(Pattern, ParseOptions(Options) | defaultOptions);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Error initializen regex '{Pattern}'.", ex);
            }
        }

        public Match Match(string input)
        {
            return Regex.Match(input);
        }

        public string Replace(string input, string replacement)
        {
            return Regex.Replace(input, replacement);
        }

        public static RegexOptions ParseOptions(IEnumerable<string> options)
        {
            var optionsValue = RegexOptions.Compiled;

            foreach (var option in options)
            {
                optionsValue |= (RegexOptions)Enum.Parse(typeof(RegexOptions), option, true);
            }

            return optionsValue;
        }
    }
}
