using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace InfinityCore.API.WorldGen
{

    abstract class ModWorldGen
    {
        public Mod mod { get; internal set; }

        public string internalName { get; internal set; }

        /// <summary>
        /// Putting this as a property in case people wanna do different world size on 64bit tML for example
        /// public int maxTileX
        /// {
        ///		get
        ///		{
        ///			if(Environment.Is64BitProcess)
        ///			{
        ///				return 168000; // extra large world
        ///			}
        ///			return 4800;
        ///		}
        /// }
        /// </summary>
        public int maxTileX { get; set; }

        /// <summary>
        /// Putting this as a property in case people wanna do different world size on 64bit tML for example
        /// public int maxTileY
        /// {
        ///		get
        ///		{
        ///			if(Environment.Is64BitProcess)
        ///			{
        ///				return 4800; // extra large world
        ///			}
        ///			return 1200;
        ///		}
        /// }
        /// </summary>
        public int maxTileY { get; set; }

        /// <summary>
        /// Visible name of your world gen type aka world type
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// Not currently implemented but will allow to display a short description of your world type
        /// </summary>
        public virtual string description { get; set; }

        /// <summary>
        /// Allow other mod to interfere with your custom world generation, could be useful if you want to allow other people 
        /// Warning, this might break with other mod
        /// </summary>
        public virtual bool allowModdedPhase { get; set; }

        /// <summary>
        /// Insert all your world gen pass here, nothing more simple than that
        /// </summary>
        /// <returns></returns>
        public virtual List<GenPass> CreateGenPassList()
        {
            return new List<GenPass>();
        }

        /// <summary>
        /// Unlock condition for the world type, will bind it to the player 
        /// </summary>
        /// <returns></returns>
        public virtual bool UnlockCondition()
        {
            return false;
        }
    }
}
