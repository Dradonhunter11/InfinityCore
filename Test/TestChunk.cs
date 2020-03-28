using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Chunks;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace InfinityCore.Test
{
    class TestChunk : ModChunk
    {
        internal static int rng = Main.rand.Next(1000000);

        public override void Draw(SpriteBatch sb, Chunk chunk)
        {
            Vector2 chunkPosition = chunk.Position * 16 + new Vector2(75 *16, 100 * 16) - Main.screenPosition;
            sb.DrawString(Main.fontDeathText, $"Test number for this chunk:{rng}", chunkPosition, Color.White);
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["number"] = rng
            };
        }

        public override void Load(TagCompound tag)
        {
            rng = tag.GetAsInt("number");
        }

        public override void NetSend(ModPacket writer)
        {
            writer.Write(rng);
        }

        public override void NetReceive(BinaryReader reader)
        {
            rng = reader.ReadInt32();
        }
    }
}
