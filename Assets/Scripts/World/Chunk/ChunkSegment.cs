using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class ChunkSegment {
        public readonly ChunkCoordinates coordinates;
        private ChunkInstance chunkInstance;
        private readonly Chunk chunk;
        private Voxel[] voxels;

        public ChunkSegment(Chunk chunk, ChunkCoordinates coordinates) {
            this.coordinates = coordinates;
            this.chunk = chunk;
        }

        /// <returns></returns>
        public void CreateChunkInstance(GameObject prefab) {
            if (chunkInstance == null) {
                chunkInstance = GameObject.Instantiate(prefab, coordinates.GetXYZ(CoordinateSpace.World),
                    new Quaternion(0, 0, 0, 1)).GetComponent<ChunkInstance>();
            }
        }

        public void SetData(Voxel[] voxels) {
            this.voxels = voxels;
        }

        public void GenerateMesh() {
            chunkInstance.meshFilter.mesh = MarchingCubes.March(voxels, new Vector3Int(16, 16, 16));
        }

        public ulong GetHashcode()
        {
            return (ulong)(coordinates.GetX(CoordinateSpace.World) + coordinates.GetZ(CoordinateSpace.World) << 32);
        }
    }
}