using InfinityCore.Worlds;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace InfinityCore.Players
{
    public class InfinityCorePlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
        }
    }

    public static class PlayerExtension
    {
        public static Chunk GetCurrentChunk(this Player self)
        {
            try
            {
                Vector2 vector = new Vector2((int)self.position.X / 16 / Chunk.sizeX, (int)self.position.Y / 16 / Chunk.sizeY);
                return InfinityCoreWorld.chunkList[$"region{((int)self.position.X / 16 / Chunk.sizeX)}{((int)self.position.Y / 16 / Chunk.sizeY)}"];
            }
            catch (Exception e)
            {
                throw new Exception("Player is in a non existent chunk");
            }
        }
    }
}
