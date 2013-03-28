using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRPG
{
    class Item
    {

        public enum itemTypes
        {
            Undefined = "Undefined",
            Armor = "Armor",
            Weapon = "Weapon"
        }

        public string itemName = "";
        /*
         * Item Types:
         * undefined
         * weapon
         * armor
        */
        public itemTypes itemType = itemTypes.Undefined;
        string test = itemTypes.Undefined.ToString();
    }
}
