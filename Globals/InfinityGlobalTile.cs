using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Pots;
using Terraria;
using Terraria.ModLoader;

namespace InfinityCore.Globals
{
    class InfinityGlobalTile : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            
            if (type == 28)
            {
                
                int potsType = Main.tile[i, j].type;
                int potsIndex = Main.tile[i, j].frameY / 18;
                //Divide by 2 to get the right potion sprite and then -1 to get the right index
                if (potsIndex > 1)
                    potsType -= 1;
                potsIndex = (int) (Math.Floor(potsType / 3f));
                //Get the actual pots type
                //Then get the coin modifier based on what type it is
                PotsRegistery.ExecuteDrop(i, j, potsIndex);
                return false;
            }
            return base.Drop(i, j, type);
        }
    }
}
