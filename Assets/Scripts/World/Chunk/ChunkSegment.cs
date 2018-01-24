using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class ChunkSegment
    {
        private readonly ChunkCoordinates coordinates;
        private readonly ChunkInstance chunkInstance;
        private readonly Chunk chunk;
        private Voxel[] voxels;

        public ChunkSegment(Chunk chunk, ChunkCoordinates coordinates) {
            this.coordinates = coordinates;
            this.chunk = chunk;
        }

        public void SetData(Voxel[] voxels) {
            this.voxels = voxels;
        }

        //public ChunkSegment(int x, int y, int z, Chunk chunk, Space space = Space.World) {
        //    this.coordinates = new ChunkCoordinates(x, y, z, space);
        //    this.chunk = chunk;
        //}

        //public void Instantiate() {
        //    GameObject.Instantiate(new GameObject(), coordinates.GetXYZ);
        //}

        public ulong GetHashcode()
        {
            return (ulong)(coordinates.GetX(Space.World) + coordinates.GetZ(Space.World) << 32);
        }
    }
}