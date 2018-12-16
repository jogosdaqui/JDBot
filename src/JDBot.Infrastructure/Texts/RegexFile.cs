using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JDBot.Infrastructure.Framework;
using Newtonsoft.Json;

namespace JDBot.Infrastructure.Texts
{
    public class RegexFile
    {
        private RegexOptions _defaultOptions;
        private readonly List<RegexInfo> _regexes = new List<RegexInfo>();

        public RegexFile(string fileName)
        {
            LoadFile(fileName);
        }

        public int RegexesCount { get => _regexes.Count; }

        public Match Match(params string[] inputs)
        {
            return MatchByTag(null, inputs);
        }

        public Match MatchByTag(string tag, params string[] inputs)
        {
            return ExecuteRegexes(tag, inputs);
        }

        public string GetValue(string groupName, params string[] inputs)
        {
            return GetValueByTag(null, groupName, inputs);
        }

        public string GetValueByTag(string tag, string groupName, params string[] inputs)
        {
            var match = MatchByTag(tag, inputs);
            return match.Success ? match.Groups[groupName].Value.Trim() : String.Empty;
        }

        public IEnumerable<string> GetResponses(params string[] inputs)
        {
            return GetResponsesByTag(null, inputs);
        }

        public IEnumerable<string> GetResponsesByTag(string tag, params string[] inputs)
        {
            var result = new List<string>();

            ExecuteRegexes(tag, inputs, (match, info) =>
            {
                foreach (var r in info.Responses)
                {
                    var resultItem = r;

                    foreach (var groupName in info.Regex.GetGroupNames())
                    {
                        resultItem = r.Replace($"${{{groupName}}}", match.Groups[groupName].Value);
                    }

                    result.Add(resultItem);
                }
            });

            return result.Distinct();
        }

        private void LoadFile(string fileName)
        {
            var content = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), Encoding.UTF8);
            var regexesInfo = JsonConvert.DeserializeObject<RegexesInfo>(content);

            _defaultOptions = regexesInfo.DefaultOptions == null ? RegexOptions.None : RegexInfo.ParseOptions(regexesInfo.DefaultOptions);

            foreach (var info in regexesInfo.Regexes)
            {
                info.Initialize(_defaultOptions);
                _regexes.Add(info);
            }
        }

        private Match ExecuteRegexes(string tag, string[] inputs, Action<Match, RegexInfo> callback = null)
        {
            var result = System.Text.RegularExpressions.Match.Empty;
            inputs = inputs.Where(i => !string.IsNullOrEmpty(i)).ToArray();
            var regexes = _regexes;

            if (!String.IsNullOrEmpty(tag))
            {
                regexes = _regexes.Where(r => r.Tags.Contains(tag)).ToList();
            }

            foreach (var r in regexes)
            {
                foreach (var input in inputs)
                {
                    Logger.Debug($"Executando a regex: {r.Pattern}...");
                    result = r.Match(input);

                    if (result.Success)
                    {
                        if (callback == null)
                            return result;
                        else
                            callback(result, r);
                    }
                }

            }

            return result;
        }

    }
}
