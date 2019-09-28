using System;
using System.Collections.Generic;
using System.IO;

namespace SearchEngine
{
    internal class RobotRules
    {
        public List<string> AllowedUrls = new List<string>();
        public List<string> DisallowedUrls = new List<string>();
        public bool Whitelist = false;
        public int Delay = 1;

        public RobotRules(string input)
        {
            string[] useragents = input.Split("User-agent:");

            foreach (string str in useragents)
            {
                if (!str.Trim().StartsWith("*")) 
                    continue;

                string[] rules = str.Split('\n');

                foreach(string rule in rules)
                {
                    if (rule.StartsWith("Disallow:"))
                        DisallowedUrls.Add(rule.Substring(9).Trim());

                    else if (rule.StartsWith("Allow:"))
                        AllowedUrls.Add(rule.Substring(6).Trim());

                    else if (rule.StartsWith("Crawl-delay:"))
                       int.TryParse(rule.Substring(12), out Delay);
                }
            }

            Whitelist = AllowedUrls.Count > 0;
        }
    }
}