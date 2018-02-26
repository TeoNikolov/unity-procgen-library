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
            this.meshData = MarchingCubes.March(voxels, new Vector3Int(16, 16, 16));
        }

        public void ApplyMesh() {
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = this.meshData.Vertices;
            mesh.normals = this.meshData.Normals;
            mesh.triangles = this.meshData.Triangles.ToArray();
            chunkInstance.meshFilter.mesh = mesh;
        }

        public void Destroy() {
            this.chunkInstance.DestroyInstance();
        }
    }
}