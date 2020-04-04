using InfinityCore.Enums;
using InfinityCore.Worlds;
using InfinityCore.Worlds.Chunk;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace InfinityCore
{
    partial class InfinityCore : Mod
    {
        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (msgType == MessageID.RequestTileData && Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(short.MaxValue);
                SyncChunkServer(packet);
                packet.Send(remoteClient, ignoreClient);
            }

            if (msgType == MessageID.WorldData)
            {
                ModPacket packet = GetPacket(short.MaxValue);
                UpdateServerChunk(packet);
                packet.Send(remoteClient, ignoreClient);
            }
            return false;
        }

        /// <summary>
        /// Initial chunk syncronization
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="whoAmI"></param>
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkMessageID messageType = (NetworkMessageID)reader.ReadByte();
            switch (messageType)
            {
                case NetworkMessageID.syncChunk:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        SyncChunkClient(reader);
                    break;
                case NetworkMessageID.regularChunkUpdate:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        UpdateClientChunk(reader);
                    break;

            }
        }


        /// <summary>
        /// Called when the client enter the server, it's basically the one big syncronization of all existing chunk in the world to the client
        /// </summary>
        private void SyncChunkServer(ModPacket packet)
        {
            packet.Write((byte)NetworkMessageID.syncChunk);
            int numberOfChunk = InfinityCoreWorld.chunkList.Count;

            for (int i = 0; i < numberOfChunk; i++)
            {
                List<Chunk> chunkList = InfinityCoreWorld.chunkList.Values.ToList();
                packet.Write(chunkList[i].chunkInternalName);
                packet.Write(chunkList[i].x);
                packet.Write(chunkList[i].y);
                chunkList[i].NetSendChunk(packet);
            }

            packet.Write(numberOfChunk);
        }

        private void UpdateServerChunk(ModPacket packet)
        {
            List<Chunk> activeChunks = InfinityCoreWorld.chunkList.Values.Where(i => i.IsActive).ToList();
            packet.Write((byte)NetworkMessageID.regularChunkUpdate);
            packet.Write(activeChunks.Count);
            for (int i = 0; i < activeChunks.Count; i++)
            {
                packet.Write(activeChunks[i].chunkInternalName);
                activeChunks[i].NetSendChunk(packet);
            }

        }

        /// <summary>
        /// Receive the initial syncronization of chunk
        /// </summary>
        private void SyncChunkClient(BinaryReader reader)
        {
            int numberOfChunk = reader.ReadInt32();

            for (int i = 0; i < numberOfChunk; i++)
            {
                string chunkInternalName = reader.ReadString();
                Chunk newChunk = new Chunk(reader.ReadInt32(), reader.ReadInt32());
                newChunk.chunkInternalName = chunkInternalName;

                newChunk.NetReceiveChunk(reader, true);
            }
        }

        private void UpdateClientChunk(BinaryReader reader)
        {
            int numberOfActiveChunk = reader.ReadInt32();
            for (int i = 0; i < numberOfActiveChunk; i++)
            {
                string chunkInternalName = reader.ReadString();
                InfinityCoreWorld.chunkList[chunkInternalName].NetReceiveChunk(reader);
            }
        }

    }
}
