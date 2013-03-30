using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
//Finish Load() code
namespace TBRPG
{
    class Program
    {
        public static LevelHandler levelHandler = new LevelHandler();
		public static CharacterHandler characterHandler = new CharacterHandler();
		static string startLevel = "level-format-prototype.lvl";

        private static void EnableDebugMode()
        {
            levelHandler.debugMode = true;
        }
        private static void DisableDebugMode()
        {
            levelHandler.debugMode = false;
        }
		private static void Save()
		{
			StreamWriter writer = new StreamWriter("Game.sav");
			
			writer.WriteLine("Start-file: " + levelHandler.curLevel);
			
			if (characterHandler.items.Count > 1)
			{
				string itemList = characterHandler.items[0];
				for (int i = 1; i < characterHandler.items.Count; i++)
				{
					itemList += "," + characterHandler.items[i];
				}
				writer.WriteLine("Items: " + itemList);
			}
			else if (characterHandler.items.Count == 1)
			{writer.WriteLine("Items: " + characterHandler.items[0]);}
			
			if (levelHandler.blockedids.Count > 1)
			{
				string blockedList = levelHandler.blockedids[0];
				for (int i = 1; i < levelHandler.blockedids.Count; i++)
				{
					blockedList += "," + levelHandler.blockedids[i];
				}
			}
			else if (levelHandler.blockedids.Count == 1)
			{writer.WriteLine("Blocked-IDs: " + levelHandler.blockedids[0]);}
			
			
			
			writer.Close();
		}
		private static bool Load()
		{
		
			if (File.Exists("Game.sav"))
			{
				StreamReader reader = new StreamReader("Game.Sav");
				String fullText = reader.ReadToEnd();
				
				characterHandler.items.Clear();
				levelHandler.blockedids.Clear();
				Console.Clear();
				
				string[] blockids = Regex.Match(fullText, "Blocked-IDs ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
				foreach (string str in blockids)
				{
					if (str != "" && str != " " && !levelHandler.blockedids.Contains(str))
					{
						levelHandler.blockedids.Add(str);
					}
				}
				
				string[] items = Regex.Match(fullText, "Items ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value.Split(',');
				foreach (string str in blockids)
				{
					if (str != "" && str != " " && !characterHandler.items.Contains(str))
					{
						characterHandler.items.Add(str);
					}
				}
				string levelName = Regex.Match(fullText, "Start-file ?: *(.+)\n", RegexOptions.IgnoreCase).Groups[1].Value;
				
				if (levelName != "" && levelName != " ")
				{
					levelHandler.LoadLevel(levelName);
				}
				else
				{
					levelHandler.LoadLevel(startLevel);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

        static void Main(string[] args)
        {
            
            string input = "";
            int choice = 0;
			if (!Load())
			{
				levelHandler.LoadLevel(startLevel);
			}

            while (true)
            {
                if (input != "")
                {
                    if (input.Equals("test", StringComparison.InvariantCultureIgnoreCase))
                    {
                        for (int i = 0; i < characterHandler.items.Count; i++)
						{
							Console.WriteLine(characterHandler.items[i]);
						}
                    }
                    else if (input.Equals("reload", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.Clear();
                        levelHandler.LoadLevel(levelHandler.curLevel);
                    }
					else if (input.Equals("reload-all", StringComparison.InvariantCultureIgnoreCase))
                    {
						characterHandler.items.Clear();
						levelHandler.blockedids.Clear();
						Console.Clear();
						levelHandler.LoadLevel(startLevel);
                    }
					else if (input.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.Clear();
						if (levelHandler.intro != "")
						{
							levelHandler.OutputToScreen(levelHandler.intro);
						}
                        levelHandler.OutputOptions();
                    }
                    else if (input.Equals("debugon", StringComparison.InvariantCultureIgnoreCase))
                    {
                        EnableDebugMode();
                        Console.WriteLine("Debugging mode enabled.");
                    }
                    else if (input.Equals("debugoff", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DisableDebugMode();
                        Console.WriteLine("Debugging mode disabled.");
                    }
                    else if (input.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine("The available commands are:");
						Console.WriteLine("Save: Saves the game.");
                        Console.WriteLine("Reload: Reloads the current level.");
						Console.WriteLine("Reload-All: Reloads everything.");
						Console.WriteLine("Clear: Clears all input.");
                        Console.WriteLine("DebugOn: Enabled debug mode.");
                        Console.WriteLine("DebugOff: Disables debug mode.");
						Console.WriteLine("Exit: Closes the program.");
                    }
					else if (input.Equals("save", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Save();
                    }
					else if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }

                    else if (int.TryParse(input, out choice) && choice > 0 && choice <= levelHandler.options.Count)
                    {
                        levelHandler.HandleResult(choice - 1);
                    }
                }

                input = Console.ReadLine();
            }

        }
    }
}
