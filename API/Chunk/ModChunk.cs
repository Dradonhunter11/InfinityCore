using System.IO;
using Terraria.World.Generation;
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

        public readonly int width = Worlds.Chunk.Chunk.sizeX;
        public readonly int height = Worlds.Chunk.Chunk.sizeY;

        /// <summary>
        /// Allow this Mod Chunk type to impact world gen
        /// </summary>
        public virtual bool ImpactWorldGen => true;

        /// <summary>
        /// Set it to true so that the world generate with this type ModChunk, can be used for example for debugging or if you want to get fancy with other library like Subworld Library
        /// </summary>
        public virtual bool CanExist => false; 

        /// <summary>
        /// This is the chunk that this instance of the mod chunk is stored currently
        /// </summary>
        public Chunk Chunk { get; internal set; }

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

        public virtual void Generate(GenerationProgress progress = null)
        {

        }

    }
}
