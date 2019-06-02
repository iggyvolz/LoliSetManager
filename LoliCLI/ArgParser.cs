using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoliCLI
{
    class ArgParser
    {
        public readonly string Command = null;
        private readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();
        private readonly List<string> Flags = new List<string>();
        public bool GetParameter(char shortVersion, string longVersion, out string value)
        {
            return Parameters.TryGetValue("-" + shortVersion, out value) || Parameters.TryGetValue("--" + longVersion, out value);
        }
        public bool HasFlag(char shortVersion, string longVersion)
        {
            return Flags.Contains("-" + shortVersion) || Flags.Contains("--" + longVersion);
        }
        public ArgParser(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            Command = args[0];
            string last = null;
            for (int i = 1; i < args.Length; i++)
            {
                if (last == null)
                {
                    last = args[i]; // Check this next time, becuase we're not sure if this is a flag or a parameter
                }
                if (last != null && args[i][0] == '-')
                {
                    // The last should have been a flag
                    Flags.Add(last);
                    last = args[i];
                }
                else
                {
                    // This is the value for the previous parameter
                    Parameters.Add(last, args[i]);
                    last = null;
                }
            }
        }
    }
}
