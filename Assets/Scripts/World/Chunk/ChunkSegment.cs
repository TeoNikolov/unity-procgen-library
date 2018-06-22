using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class ChunkSegment {
        public readonly ChunkCoordinates3D coordinates;
        private readonly Chunk chunk;
        private ChunkInstance chunkInstance;
        private Voxel[] voxels;
        private MeshData meshData;

        public ChunkSegment(Chunk chunk, ChunkCoordinates3D coordinates) {
            this.coordinates = coordinates;
            this.chunk = chunk;
        }

        /// <summary>
        /// Instantiates a Chunk game object within Unity at this chunk segment's coordinates.
        /// The methos links the resulting ChunkInstance script with this instance of the chunk segment.
        /// </summary>
        /// <param name="prefab">The Chunk prefab to instantiate</param>
        public void CreateChunkInstance(GameObject prefab) {
            if (this.chunkInstance == null) {
                this.chunkInstance = GameObject.Instantiate(prefab,
                    this.coordinates.GetXYZ(CoordinateSpace.World),
                    new Quaternion(0, 0, 0, 1)).GetComponent<ChunkInstance>();
            }
        }

        public void SetData(Voxel[] voxels) {
            this.voxels = voxels;
        }

        public void GenerateMesh() {
            this.meshData = MarchingCubes.March(voxels, new Vector3Int(16, 16, 16));
        }

        public void ApplyMesh() {
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = this.meshData.Vertices;
            mesh.normals = this.meshData.Normals;
            mesh.triangles = this.meshData.Triangles.ToArray();
            this.chunkInstance.meshFilter.mesh = mesh;
        }

        public void Destroy() {
            this.chunkInstance.DestroyInstance();
        }
    }
}