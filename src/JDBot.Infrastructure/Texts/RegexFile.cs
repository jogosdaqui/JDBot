using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JDBot.Infrastructure.Texts
{
    public class RegexFile
    {
        private readonly List<Regex> _regexes = new List<Regex>();

        public RegexFile(string fileName)
        {
            LoadFile(fileName);
        }

        public Match Match(params string[] inputs)
        {
            Match result = null;
            inputs = inputs.Where(i => !string.IsNullOrEmpty(i)).ToArray();

            foreach (var r in _regexes)
            {
                foreach (var input in inputs)
                {
                    result = r.Match(input);

                    if (result.Success)
                        return result;
                }

            }

            return result;
        }

        public string GetValue(string groupName, params string[] inputs)
        {
            var match = Match(inputs);
            return match.Success ? match.Groups[groupName].Value.Trim() : String.Empty;
        }

        private void LoadFile(string fileName)
        {
            var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), Encoding.UTF8);

            foreach(var line in lines)
            {
                _regexes.Add(new Regex(line, RegexOptions.Compiled | RegexOptions.IgnoreCase));
            }
        }
    }
}
