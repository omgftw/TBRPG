using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TBRPG
{

    class LevelHandler
    {
        public bool debugMode = false;
        public string curLevel = "";
        public string intro = "";
        public List<Option> options = new List<Option>();
		public List<string> blockedids = new List<string>();

        public void OutputToScreen(string output)
        {
            output = Regex.Replace(output, @"(?<!\\)\\n", "\n").Replace("\\\\", "\\");
            output = "\n" + output + "\n";
            Console.WriteLine(output);
        }

        public void LoadLevel(string fileLocation)
        {
            if (File.Exists(fileLocation))
            {
				fileLocation = fileLocation.Trim(Path.GetInvalidFileNameChars());
				fileLocation = fileLocation.Trim(Path.GetInvalidPathChars());
                curLevel = fileLocation;
                Console.Clear();
                options.Clear();

                StreamReader reader = new StreamReader(fileLocation);
                string fullText = reader.ReadToEnd();
                bool optionFound = false;

                //Parse Intro
                intro = Regex.Match(fullText, "Intro ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                
                foreach (Match mat in Regex.Matches(fullText, "Option ?:.*?{(.+?)}", RegexOptions.Singleline | RegexOptions.IgnoreCase))
                {
                    Option opt = new Option();
                    optionFound = true;
                    string curOpt = mat.Groups[1].Value;

                    opt.id = Regex.Match(curOpt, "[^-]ID ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    opt.optionText = Regex.Match(curOpt, "Option-text ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    opt.resultText = Regex.Match(curOpt, "Result-text ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    opt.loadFile = Regex.Match(curOpt, "Load ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    opt.blockedText = Regex.Match(curOpt, "Blocked-text ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    //opt.requiredItem = Regex.Match(curOpt, "Required-item ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
                    opt.missingRequiredItemsText = Regex.Match(curOpt, "Missing-required-items-text ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;

					
					string[] requiredItems = Regex.Match(curOpt, "Required-items ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
                    foreach (string str in requiredItems)
                    {
                        if (str != "" && str != " ")
                        {
                            opt.requiredItems.Add(str.TrimStart().TrimEnd());
                        }
                    }
                    string[] blockids = Regex.Match(curOpt, "Block-ID ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
                    foreach (string str in blockids)
                    {
                        if (str != "" && str != " ")
                        {
                            opt.blockids.Add(str.TrimStart().TrimEnd());
                        }
                    }
                    string[] unblockids = Regex.Match(curOpt, "Unblock-ID ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
                    foreach (string str in unblockids)
                    {
                        if (str != "" && str != " ")
                        {
                            opt.unblockids.Add(str.TrimStart().TrimEnd());
                        }
                    }
                    string[] giveItems = Regex.Match(curOpt, "Give-items ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
                    foreach (string str in giveItems)
                    {
                        if (str != "" && str != " ")
                        {
                            opt.giveItems.Add(str.TrimStart().TrimEnd());
                        }
                    }
                    opt.resultText = Regex.Match(curOpt, "Result-text ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;

                    options.Add(opt);
                }

               //final handling
                if (intro != "")
                {
                    Console.WriteLine(Regex.Replace(intro, @"(?<!\\)\\n", "\n").Replace("\\\\", "\\") + "\n");
                }
                else if (debugMode)
                {
                    Console.WriteLine("Warning: No intro found.");
                }
                if (optionFound)
                {
                    OutputOptions();
                }
                else if (debugMode)
                {
                    Console.WriteLine("Error: No options found.");
                }

                reader.Close();
            }
			else if (debugMode)
			{
				Console.WriteLine("Error: Attempted to load a file that did not exist: " + fileLocation);
			}
        }

        public void OutputOptions()
        {
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine((i + 1).ToString() + ". " + options[i].optionText.Replace("\\n", "\n"));
            }
        }
		
		public void HandleResult(int id)
		{
			Option curOpt = options[id];
            bool hasReqItems = true;
			
			if (!blockedids.Contains(curOpt.id))
			{

                for (int i = 0; i < curOpt.requiredItems.Count; i++)
                {
                    if (!Program.characterHandler.hasItem(curOpt.requiredItems[i]))
                    {
                        hasReqItems = false;
                    }
                }


                if (hasReqItems)
                {
                    for (int i = 0; i < curOpt.blockids.Count; i++)
                    {
                        if (curOpt.blockids[i] != "" && curOpt.blockids[i] != " " && !blockedids.Contains(curOpt.blockids[i]))
                        {
                            blockedids.Add(curOpt.blockids[i]);
                        }
                    }
                    for (int i = 0; i < curOpt.unblockids.Count; i++)
                    {
                        if (curOpt.unblockids[i] != "" && curOpt.unblockids[i] != " ")
                        {
                            blockedids.Remove(curOpt.unblockids[i]);
                        }
                    }

                    for (int i = 0; i < curOpt.giveItems.Count; i++)
                    {
                        if (curOpt.giveItems[i] != "" && curOpt.giveItems[i] != " ")
                        {
                            Program.characterHandler.items.Add(curOpt.giveItems[i]);
                        }
                    }

                    if (curOpt.loadFile != "" && curOpt.loadFile != " ")
                    {
                        LoadLevel(curOpt.loadFile);
                        if (curOpt.resultText != "" && curOpt.resultText != " ")
                        {
                            OutputToScreen(curOpt.resultText);
                        }
                    }
                    else if (curOpt.resultText != "" && curOpt.resultText != " ")
                    {
                        OutputToScreen(curOpt.resultText);
                    }
                }
                else
                {
                    if (curOpt.missingRequiredItemsText != "" && curOpt.missingRequiredItemsText != " ")
                    {
                        string output = curOpt.missingRequiredItemsText;

                        for (int i = 0; i < curOpt.requiredItems.Count; i++)
                        {
                            output = output.Replace("%" + (i+1).ToString() + "%", curOpt.requiredItems[i]);
                        }

                        OutputToScreen(output);
                    }
                }
			}
			else
			{
                OutputToScreen(curOpt.blockedText);
			}
			
			
			
		}


    }
}
