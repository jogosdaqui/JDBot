using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.IO
{
    public static class UrlFileParser
    {
        public static UrlFile Parse(string filename)
        {
            var urlFile = new UrlFile();
            var items = new List<UrlFileItem>();
            var lines = File.ReadAllLines(filename)
                            .Where(l => !l.StartsWith("#", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty(l))
                            .ToArray();

             urlFile.JekyllRootFolder = lines[0];

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');

                var item = new UrlFileItem
                {
                    Config = new PostConfig
                    {
                        Author = parts[0],
                        Date = DateTime.Parse(parts[1])
                    },
                    Url = parts[2]
                };

                items.Add(item);
            }

            urlFile.Items = items;

            return urlFile;
        }
    }
}