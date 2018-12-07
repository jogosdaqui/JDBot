using System;
using System.Diagnostics;
using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.Git
{
    public class GitProxy : IGitProxy
    {
        public void Init()
        {
            Run("init");
        }

        public void Add(string filter)
        {
            Run($"add {filter}");
        }

        public void Commit(string message, string description = null)
        {
            if(String.IsNullOrEmpty(description))
                Run($"commit -m\"{message}\"");
            else
                Run($"commit -m\"{message}\" -m\"{description}\"");
        }

        public void Checkout(string branch)
        {
            Run($"checkout {branch}");
        }

        public void CheckoutNewBranch(string branch)
        {
            Run($"checkout -b {branch}");
        }

        public void Push()
        {
            Run("push");
        }

        public void PushTags()
        {
            Run("push --tags");
        }

        public static void Run(string command)
        {
            var startInfo = new ProcessStartInfo("git", command)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var ps = Process.Start(startInfo);
            ps.WaitForExit();

            if (ps.ExitCode != 0)
            {
                var error = ps.StandardError.ReadToEnd();

                if(String.IsNullOrEmpty(error))
                    error = ps.StandardOutput.ReadToEnd();

                throw new InvalidOperationException($"Error executing git command: {error}");
            }
        }
    }
}
