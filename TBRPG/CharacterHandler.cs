using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRPG
{
    class CharacterHandler
    {
        public List<string> items = new List<string>();

        public bool hasItem(string itemname)
        {


            bool returnVal = false;
            itemname = itemname.ToLower();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ToLower() == itemname)
                {
                    returnVal = true;
                }
            }

            return returnVal;
        }
    }
}
