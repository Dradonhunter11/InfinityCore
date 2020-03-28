using System.IO;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace InfinityCore.API.Chunks
{
    public class ModChunk
    {
        /// <summary>
        /// Be careful, global chunk can be very laggy 
        /// </summary>
        public virtual bool IsGlobal => false;

        public readonly int WIDTH = Worlds.Chunk.Chunk.SIZE_X;
        public readonly int HEIGHT = Worlds.Chunk.Chunk.SIZE_Y;

        public Chunk chunk { get; internal set; }

        public virtual void Initialize()
        {

        }

        public virtual TagCompound Save()
        {
            return new TagCompound();
        }

        public virtual void Load(TagCompound tag)
        {
            return;
        }

        public virtual void NetSend(ModPacket writer)
        {
            return;
        }

        public virtual void NetReceive(BinaryReader reader)
        {
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
        {
            return;
        }

        /// <summary>
        /// Allow you to execute special drawing in the chunk, for example special effect when someone in a chunk
        /// </summary>
        public virtual void Draw(SpriteBatch sb, Worlds.Chunk.Chunk chunk)
        {

        }

    }
}
