﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SearchEngine
{
    internal class RobotRules
    {
        public List<string> AllowedUrls = new List<string>();
        public List<string> DisallowedUrls = new List<string>();
        bool whitelist = false;
        public int Delay = 2;

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
                        Delay = int.Parse(rule.Substring(12));
                }

                Console.WriteLine("Disallowed:");
                foreach (string line in DisallowedUrls)
                    Console.WriteLine(line);

                Console.WriteLine("Allowed:");
                if (AllowedUrls.Count != 0)
                {
                    foreach(string line in AllowedUrls)
                        Console.WriteLine(line);
                }

                Console.WriteLine("Delay: " + Delay);
                Console.ReadKey();
            }

            if(AllowedUrls.Count > 0)
            {
                whitelist = true;
            }
            else
            {
                whitelist = false;
            }
        }
    }
}