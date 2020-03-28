using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityCore.Enums
{
    enum NetworkMessageID : byte
    {
        /// <summary>
        /// Happen when you join the world, it sync all the chunk between client and server
        /// </summary>
        syncChunk = 0,
        /// <summary>
        /// Happen when updating the chunk, also send mod chunk update
        /// </summary>
        regularChunkUpdate = 1
    }
}
