using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Pots.DropTable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace InfinityCore.API.Pots
{
    public abstract class ModPots : ModTile
    {
        public abstract int width { get; }
        public abstract int height { get; }

        public virtual void SetExtraDefaults(ref TileObjectData data)
        {
            return;
        }

        public sealed override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;

            SetExtraDefaults(ref TileObjectData.newTile);
            PotsRegistery.RegisterDefaultsPotsDrop(Type);
            RegisterPotsDrop(PotsRegistery.ModifyPotsDrop(Type));
            
            TileObjectData.addTile(Type);
            base.SetDefaults();
        }

        /// <summary>
        /// If vanilla pots, here is the order:<br />
        /// [0] = PortalCoin<br />
        /// IF Dungeon Pots <br />
        /// [1] = GoldenKey <br />
        /// Everything after is index + 1 for dungeon pots <br />
        /// [1] = PotionDrop<br />
        /// [2] = Wormhole_MP<br />
        /// [3] = DefaultDrop See note bellow<br />
        ///<br /> <br />
        ///
        /// DefaultDrop info<br />
        /// Have 8 possible drop, which are ran trough a randomizer of 7 (6 if in the underworld or in hardmode)<br />
        /// 0 = Heart drop<br />
        /// 1 = Torch drop, if a pots is a biome pots, it will drop the torch matching it's biome<br />
        /// 2 = Ammo drop<br />
        /// 3 = Healing pots drop<br />
        /// 4 = bomb drop<br />
        /// 5 (underworld and hardmode, otherwise this is 6) =  coin drop<br />
        /// 5 (if not in hardmode or in underworld) = Rope
        /// </summary>
        /// <param name="potsType"></param>
        /// <returns></returns>
        public virtual void RegisterPotsDrop(List<PotsDrop> dropList)
        {
            return;
        }

        public sealed override bool Drop(int x, int y)
        {
            if (Terraria.WorldGen.destroyObject)
                return false;

            for (int m = x; m < x + width; m++) {
                for (int n = y; n < y + width; n++) {
                    if (Main.tile[m, n].type == 28 && Main.tile[m, n].active())
                    {
                        Main.tile[m, n].type = 0;
                        Main.tile[m, n].active(false);
                    }
                        
                }
            }

            foreach (PotsDrop potsDrop in PotsRegistery.ModifyPotsDrop(Type))
            {
                if (potsDrop.successFunc(x, y))
                {
                    potsDrop.ExecuteDrop(x, y);
                    return false;
                }
                Terraria.WorldGen.destroyObject = true;
            }

            Terraria.WorldGen.destroyObject = true;

            return false;
        }
    }
}
