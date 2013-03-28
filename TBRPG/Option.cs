using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRPG
{
    class Option
    {
        public string id = "";
        public string optionText = "";
        public string resultText = "";
        public string loadFile = "";
        public List<string> blockids = new List<string>();
        public List<string> unblockids = new List<string>();
        public string blockedText = "";
        public List<string> giveItems = new List<string>();
        public List<string> requiredItems = new List<string>();
        public string missingRequiredItemsText = "";

    }
}
