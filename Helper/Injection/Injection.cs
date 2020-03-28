using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Entity = Terraria.Entity;

namespace InfinityCore.Helper.Injection
{
	static partial class Injection
	{
		internal static void Load()
        {
            On.Terraria.GameContent.UI.States.UIWorldSelect.UpdateWorldsList += Detour_UpdateWorldList;
            On.Terraria.GameContent.UI.States.UIWorldSelect.OnInitialize += Detour_OnInitializeWorldSelectUI;
            //On.Terraria.GameContent.UI.States.UIWorldSelect.UpdateFavoritesCache += Detour_UpdateFavoritesCache;
            On.Terraria.GameContent.UI.States.UIWorldSelect.Draw += Detour_UIWorldSelectDraw;
            UpdateWorldUISelect_Hook += Detour_UIWorldSelectUpdate;
        }



        internal static void Unload()
		{
			On.Terraria.GameContent.UI.States.UIWorldSelect.UpdateWorldsList -= Detour_UpdateWorldList;
			On.Terraria.GameContent.UI.States.UIWorldSelect.OnInitialize -= Detour_OnInitializeWorldSelectUI;
			//On.Terraria.GameContent.UI.States.UIWorldSelect.UpdateFavoritesCache -= Detour_UpdateFavoritesCache;
            On.Terraria.GameContent.UI.States.UIWorldSelect.Draw -= Detour_UIWorldSelectDraw;

		}
    }
}
