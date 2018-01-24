using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class Chunk
    {
        /// <summary>
        /// The number of chunk segments a chunk has vertically.
        /// </summary>
        public static readonly int height = 8;
        private List<ChunkSegment> chunkSegments;

        public Chunk(Vector2Int xz, Space space = Space.World) : this(xz.x, xz.y, space) { }

        public Chunk(int x, int z, Space space = Space.World) {
            if (space == Space.World) {
                x >>= 4;
                z >>= 4;
            }

            chunkSegments = new List<ChunkSegment>(height);

            for (int y = 0; y < height; y++) {
                ChunkCoordinates coordinates = new ChunkCoordinates(x, y * 16, z, Space.Chunk);
                ChunkSegment segment = new ChunkSegment(this, coordinates);
                chunkSegments.Add(segment);
            }
        }
    }
}